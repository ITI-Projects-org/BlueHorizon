import {
  Component,
  OnInit,
  OnDestroy,
  HostListener,
  Inject,
  PLATFORM_ID,
  ChangeDetectorRef,
} from '@angular/core';
import { Router, NavigationEnd, RouterModule } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { filter } from 'rxjs/operators';
import { AuthService } from '../../Services/auth.service';
import { Messages } from '../../Services/messages';
import { ChatService } from '../../Services/chat.service';
import { InboxItem } from '../../Models/chat.models';
import { Subscription } from 'rxjs';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar implements OnInit, OnDestroy {
  _isLoggedIn: boolean = false;

  isHomePage: boolean = false;
  isScrolled = false;

  showPopup = false;
  recentChats: InboxItem[] = [];
  isLoadingMessages = false;
  hasMessagesError = false;
  messagesErrorMessage = '';
  messagesRetryCount = 0;
  maxRetries = 3;

  userRole: string = '';
  isOwner: boolean = false;
  isTenant: boolean = false;
  isAdmin: boolean = false;

  private isBrowser: boolean;
  private hubConnection?: signalR.HubConnection;
  private messageSubscription?: Subscription;

  constructor(
    private router: Router,
    private authService: AuthService,
    private messagesService: Messages,
    private chatService: ChatService,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit(): void {
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        this.isHomePage = this.router.url === '/';
        this._isLoggedIn = this.authService.isLoggedIn();
        this.updateUserRole();
      });

    this._isLoggedIn = this.authService.isLoggedIn();
    this.updateUserRole();

    if (this._isLoggedIn) {
      this.fetchRecentChatsForPopupWithRetry();
    }
  }

  ngOnDestroy(): void {
    if (this.messageSubscription) {
      this.messageSubscription.unsubscribe();
    }
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  updateUserRole(): void {
    this.userRole = this.authService.getCurrentUserRole() || '';
    this.isOwner = this.userRole === 'Owner';
    this.isTenant = this.userRole === 'Tenant';
    this.isAdmin = this.userRole === 'Admin';
  }

  canSeeAddUnit(): boolean {
    return this._isLoggedIn && this.isOwner;
  }

  canSeeMyBookings(): boolean {
    return this._isLoggedIn && (this.isTenant || this.isOwner);
  }

  canSeePendingRequests(): boolean {
    return this._isLoggedIn && this.isOwner;
  }

  canSeeMessages(): boolean {
    return this._isLoggedIn;
  }

  canSeeProfile(): boolean {
    return this._isLoggedIn;
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    if (this.isBrowser) {
      this.isScrolled = window.scrollY > 50;
      this.updateNavbarStyle();
    }
  }

  updateNavbarStyle(): void {
    const navbar = document.querySelector('.navbar') as HTMLElement;
    if (navbar) {
      if (this.isScrolled) {
        navbar.classList.add('scrolled');
      } else {
        navbar.classList.remove('scrolled');
      }
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    event.stopPropagation();

    if (this.showPopup && this._isLoggedIn) {
      this.fetchRecentChatsForPopup();
    }

    const popup = document.querySelector('.messages-popup');
    const icon = document.querySelector('.messages-icon');

    if (popup && icon) {
      if (!popup.contains(event.target as Node) && !icon.contains(event.target as Node)) {
        this.showPopup = false;
      }
    }
  }

  toggleMessagesPopup(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.showPopup = !this.showPopup;

    if (this.showPopup && this._isLoggedIn) {
      this.fetchRecentChatsForPopup();
    }
  }

  fetchRecentChatsForPopupWithRetry(): void {
    this.isLoadingMessages = true;
    this.hasMessagesError = false;
    this.messagesErrorMessage = '';

    this.messagesService.getInboxMessages().subscribe({
      next: (inboxItems: InboxItem[]) => {
        this.recentChats = inboxItems.slice(0, 5);
        this.isLoadingMessages = false;
        this.messagesRetryCount = 0;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching recent chats:', error);
        this.isLoadingMessages = false;
        this.hasMessagesError = true;
        this.messagesErrorMessage = 'Failed to load recent messages';
        this.messagesRetryCount++;
        this.cdr.detectChanges();
      }
    });
  }

  retryLoadMessages(): void {
    if (this.messagesRetryCount < this.maxRetries) {
      this.fetchRecentChatsForPopupWithRetry();
    }
  }

  fetchRecentChatsForPopup(): void {
    if (this._isLoggedIn) {
      this.fetchRecentChatsForPopupWithRetry();
    }
  }

  navigateToChat(otherUserId: string): void {
    this.showPopup = false;
    this.router.navigate(['/chat'], { queryParams: { userId: otherUserId } });
  }

  startSignalRConnection(): void {
    if (this.isBrowser && this._isLoggedIn) {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('https://localhost:7083/chathub', {
          accessTokenFactory: () => this.authService.getToken() || ''
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Debug)
        .build();

      this.hubConnection.start()
        .then(() => {
          console.log('SignalR Connected');
        })
        .catch(err => {
          console.error('SignalR Connection Error:', err);
        });

             this.hubConnection.on('ReceiveMessage', (messageData) => {
         const message: InboxItem = {
           otherUserId: messageData.senderId,
           otherUserName: messageData.senderName || 'Unknown User',
           lastMessageContent: messageData.messageContent,
           lastMessageTimestamp: new Date(messageData.timeStamp),
           unreadCount: 1,
           lastMessageIsRead: false
         };

         this.recentChats = [message, ...this.recentChats.filter(chat => chat.otherUserId !== message.otherUserId)].slice(0, 5);
         this.fetchRecentChatsForPopup();
       });
    }
  }

  sendMessage(receiverId: string, messageContent: string): void {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendMessage', receiverId, messageContent)
        .catch(err => {
          console.error('Error sending message:', err);
        });
    }
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  logout(): void {
    this.authService.logout();
    this._isLoggedIn = false;
    this.updateUserRole();
    this.router.navigate(['/']);
  }
}


