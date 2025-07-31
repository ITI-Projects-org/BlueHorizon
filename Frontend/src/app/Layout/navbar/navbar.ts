import {
  Component,
  HostListener,
  OnInit, // تأكد من وجود OnInit هنا
  OnDestroy,
  Inject,
  PLATFORM_ID,
} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common'; // تأكد من وجود CommonModule هنا
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter } from 'rxjs/operators'; // تأكد من استيراد filter من 'rxjs/operators'

// Import services related to user and messages
import { AuthService } from '../../Services/auth.service';
import { Messages } from '../../Services/messages';
import { InboxItem } from '../../Models/chat.models';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

@Component({
  selector: 'app-navbar',
  // تأكد من وجود CommonModule في الـ imports هنا
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar implements OnInit, OnDestroy { // تأكد من تطبيق الواجهتين OnInit, OnDestroy
  isScrolled = false;
  private isBrowser: boolean;
  isHomePage: boolean = false;
  _isLoggedIn: boolean = false; // **هذه الخاصية هي الأهم لحالة تسجيل الدخول**

  // Message and popup related properties and logic
  showPopup: boolean = false;
  recentChats: InboxItem[] = [];
  unreadMessagesCount: number = 0;
  hubConnection!: HubConnection;
  currentUserId: string | null = null;

  // Role-based navigation properties
  userRole: string = '';
  isOwner: boolean = false;
  isTenant: boolean = false;
  isAdmin: boolean = false;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private router: Router,
    private authService: AuthService,
    private messagesService: Messages
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.currentUserId = this.authService.getCurrentUserId();

    if (this.isBrowser) {
      // الاستماع لتغييرات المسار لتحديث isHomePage و _isLoggedIn
      this.router.events
        .pipe(filter((event) => event instanceof NavigationEnd))
        .subscribe((event: NavigationEnd) => {
          this.isHomePage =
            event.urlAfterRedirects === '/' ||
            event.urlAfterRedirects.startsWith('/home');
          this.updateNavbarStyle();
          this._isLoggedIn = this.authService.isLoggedIn(); // **تحديث حالة تسجيل الدخول عند تغيير المسار**
          this.updateUserRole(); // Update role when route changes
        });
    }
  }

  ngOnInit(): void {
    if (this.isBrowser) {
      window.addEventListener('scroll', this.onWindowScroll.bind(this));
      document.addEventListener('click', this.onDocumentClick.bind(this));
    }

    // **تحديث حالة تسجيل الدخول فورًا عند تهيئة المكون**
    this._isLoggedIn = this.authService.isLoggedIn();
    this.updateUserRole(); // Update role on component initialization

    // جلب المحادثات وبدء SignalR فقط إذا كان المستخدم مسجلًا للدخول
    if (this._isLoggedIn) {
      this.fetchRecentChatsForPopup();
      this.startSignalRConnectionForNavbar();
    }
  }

  ngOnDestroy(): void {
    if (this.isBrowser) {
      window.removeEventListener('scroll', this.onWindowScroll.bind(this));
      document.removeEventListener('click', this.onDocumentClick.bind(this));
    }
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      this.hubConnection
        .stop()
        .then(() => console.log('Navbar SignalR connection stopped.'))
        .catch((err) => console.error(err));
    }
  }

  // Role-based navigation methods
  private updateUserRole(): void {
    if (this._isLoggedIn) {
      this.userRole = this.authService.getCurrentUserRole() || '';
      this.isOwner = this.userRole === 'Owner';
      this.isTenant = this.userRole === 'Tenant';
      this.isAdmin = this.userRole === 'Admin';
    } else {
      this.userRole = '';
      this.isOwner = false;
      this.isTenant = false;
      this.isAdmin = false;
    }
  }

  // Navigation visibility methods
  canSeeAddUnit(): boolean {
    return this._isLoggedIn && this.isOwner;
  }

  canSeeMyBookings(): boolean {
    return this._isLoggedIn && this.isTenant;
  }

  canSeePendingRequests(): boolean {
    return this._isLoggedIn && this.isAdmin;
  }

  canSeeMessages(): boolean {
    return this._isLoggedIn; // All authenticated users can see messages
  }

  canSeeProfile(): boolean {
    return this._isLoggedIn; // All authenticated users can see profile
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    if (this.isBrowser) {
      this.isScrolled = window.scrollY > 50;
      this.updateNavbarStyle();
    }
  }

  private updateNavbarStyle() {
    const navbarElement = document.querySelector('.navbar');
    if (navbarElement) {
      if (this.isHomePage && !this.isScrolled) {
        navbarElement.classList.remove('scrolled');
      } else {
        navbarElement.classList.add('scrolled');
      }
    }
  }

  // Popup logic
  toggleMessagePopup(event?: Event): void {
    if (event) {
      event.stopPropagation(); // Prevent event from bubbling up the DOM
    }
    this.showPopup = !this.showPopup;
    if (this.showPopup && this._isLoggedIn) { // استخدام _isLoggedIn
      this.fetchRecentChatsForPopup(); // Fetch chats only when opening the popup
    }
  }

  onDocumentClick(event: MouseEvent): void {
    if (this.showPopup && this.isBrowser) {
      const popupElement = document.querySelector('.message-popup');
      const iconElement = document.querySelector('.message-icon-container');

      // Ensure elements exist before using contains
      if (popupElement && iconElement) {
        // If click is outside both the popup and the icon, close the popup
        if (
          !popupElement.contains(event.target as Node) &&
          !iconElement.contains(event.target as Node)
        ) {
          this.showPopup = false;
        }
      }
    }
  }

  fetchRecentChatsForPopup(): void {
    this.messagesService.getInboxMessages().subscribe({
      next: (inboxItems: InboxItem[]) => {
        this.recentChats = inboxItems.slice(0, 5); // Display only top 5 recent chats in popup
        this.unreadMessagesCount = inboxItems.reduce(
          (sum, chat) => sum + chat.unreadCount,
          0
        );
        console.log('Recent chats fetched for popup:', this.recentChats);
      },
      error: (err) => {
        console.error('Error fetching recent chats for navbar:', err);
        this.recentChats = [];
        this.unreadMessagesCount = 0;
      },
    });
  }

  goToChat(otherUserId: string): void {
    this.router.navigate(['/chat'], { queryParams: { userId: otherUserId } });
    this.showPopup = false; // Close the popup after navigating to chat
  }

  private startSignalRConnectionForNavbar(): void {
    const token = this.authService.getToken();
    if (!token) {
      console.error('No JWT token found for Navbar SignalR connection.');
      return;
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7083/chathub', {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Navbar SignalR connection started.');
      })
      .catch((err) =>
        console.error('Error while starting SignalR connection: ' + err)
      );

    this.hubConnection.on('ReceiveMessage', () => {
      this.fetchRecentChatsForPopup(); // Refresh recent chats on new message
    });
  }

  // هذه الدالة تم استبدالها بالخاصية _isLoggedIn
  // لكن إذا كنت لا تزال تستخدمها في أماكن أخرى غير HTML، اتركها.
  // إذا كنت تستخدمها فقط في HTML، فيمكنك حذفها بعد التعديل.
  // isLoggedIn(): boolean {
  //   return this.authService.isLoggedIn();
  // }

  getCurrentUserName(): string | null {
    return this.authService.getCurrentUserName();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
    this._isLoggedIn = false; // **تحديث الحالة عند تسجيل الخروج**
    this.updateUserRole(); // Update role after logout
  }
}
