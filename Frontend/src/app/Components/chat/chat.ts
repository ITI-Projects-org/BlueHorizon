import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked, Inject, PLATFORM_ID, NgZone, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../Services/auth.service';
import { Messages } from '../../Services/messages';
import { ChatService } from '../../Services/chat.service';
import { InboxItem, ChatMessage } from '../../Models/chat.models';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html',
  styleUrls: ['./chat.css']
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {

  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  currentUserId: string | null = null;
  newMessageContent: string = '';

  recentChats: InboxItem[] = [];
  selectedChatUserId: string | null = null;
  selectedChatUser: InboxItem | null = null;
  messages: ChatMessage[] = [];

  private isBrowser: boolean;
  private shouldScrollToBottom: boolean = false;

  constructor(
    private authService: AuthService,
    private messagesService: Messages,
    private chatService: ChatService,
    private route: ActivatedRoute,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.currentUserId = this.authService.getCurrentUserId();
    if (!this.currentUserId && this.isBrowser) {
      console.error('Current user ID is not available. User might not be logged in or token is invalid. Redirecting to login.');
      this.router.navigate(['/login']);
    }
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn() && this.isBrowser) {
      console.log('User not logged in, cannot initialize chat. Redirecting to login.');
      this.router.navigate(['/login']);
      return;
    }

    console.log('🚀 ChatComponent initialized for user:', this.currentUserId);

    this.fetchRecentChats();

    if (this.isBrowser) {
      this.startSignalRConnection();
    }

    this.route.queryParams.subscribe(params => {
      const userIdFromUrl = params['userId'];
      if (userIdFromUrl && userIdFromUrl !== this.selectedChatUserId) {
        this.selectChat(userIdFromUrl);
      } else if (!userIdFromUrl && !this.selectedChatUserId && this.recentChats.length > 0) {
        this.selectChat(this.recentChats[0].otherUserId);
      }
    });
  }

  ngAfterViewChecked(): void {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  ngOnDestroy(): void {
    console.log('🛑 ChatComponent destroyed, stopping SignalR connection');
    this.chatService.stopConnection();
  }

  private startSignalRConnection(): void {
    console.log('🔌 Starting SignalR connection...');
    this.chatService.startConnection();

    // 🔴 إضافة debugging أكثر تفصيلاً
    this.chatService.messages$.subscribe({
      next: (messageData: ChatMessage) => {
        console.log('📨 Component received message from service:', messageData);
        console.log('🔍 Current selected chat user:', this.selectedChatUserId);
        console.log('🔍 Current user ID:', this.currentUserId);

        // التحقق من الرسالة
        const isRelevantMessage = (
          (this.selectedChatUserId === messageData.senderId && this.currentUserId === messageData.receiverId) ||
          (this.selectedChatUserId === messageData.receiverId && this.currentUserId === messageData.senderId)
        );

        console.log('🔍 Is message relevant to current chat?', isRelevantMessage);

        if (isRelevantMessage) {
          console.log('✅ Adding message to current chat');
          console.log('📊 Messages before:', this.messages.length);

          this.ngZone.run(() => {
            this.messages = [...this.messages, messageData];
            console.log('📊 Messages after:', this.messages.length);
            this.shouldScrollToBottom = true;
            this.cdr.detectChanges();
            console.log('🔄 UI updated');
          });
        }

        // تحديث قائمة المحادثات
        this.ngZone.run(() => {
          this.fetchRecentChats();
          this.cdr.detectChanges();
        });
      },
      error: (error) => {
        console.error('❌ Error in messages subscription:', error);
      }
    });
  }

  fetchRecentChats(): void {
    this.messagesService.getInboxMessages().subscribe({
      next: (inboxItems: InboxItem[]) => {
        this.ngZone.run(() => {
          this.recentChats = inboxItems;
          if (this.selectedChatUserId) {
              this.selectedChatUser = this.recentChats.find(chat => chat.otherUserId === this.selectedChatUserId) || null;
              if (this.messages.length === 0) {
                this.loadMessagesForSelectedChat();
              }
          } else if (this.recentChats.length > 0 && !this.selectedChatUserId) {
              this.selectChat(this.recentChats[0].otherUserId);
          }
          this.cdr.detectChanges();
        });
      },
      error: (err) => {
        console.error('Error fetching recent chats:', err);
        this.ngZone.run(() => {
            this.recentChats = [];
            this.cdr.detectChanges();
        });
      }
    });
  }

  selectChat(otherUserId: string): void {
    console.log('💬 Selecting chat with user:', otherUserId);
    this.selectedChatUserId = otherUserId;
    this.selectedChatUser = this.recentChats.find(chat => chat.otherUserId === otherUserId) || null;
    this.messages = [];
    this.newMessageContent = '';

    this.loadMessagesForSelectedChat();
  }

  private loadMessagesForSelectedChat(): void {
    if (this.selectedChatUserId) {
      console.log('📥 Loading chat history with:', this.selectedChatUserId);
      this.messagesService.getChatHistory(this.selectedChatUserId).subscribe({
        next: (messages: ChatMessage[]) => {
          this.ngZone.run(() => {
            this.messages = messages;
            this.shouldScrollToBottom = true;
            console.log("📚 Loaded messages count:", this.messages.length);
            this.cdr.detectChanges();
          });
        },
        error: (err) => {
          console.error('Error fetching conversation messages:', err);
          this.ngZone.run(() => {
            this.messages = [];
            this.cdr.detectChanges();
          });
        }
      });
    }
  }

  sendMessage(): void {
    if (!this.newMessageContent.trim() || !this.selectedChatUserId) {
      console.log('❌ Cannot send empty message or no chat selected');
      return;
    }

    console.log('📤 Sending message...');
    console.log('📤 To:', this.selectedChatUserId);
    console.log('📤 Content:', this.newMessageContent.trim());
    console.log('📤 Connection state:', this.chatService.getConnectionState());

    this.chatService.sendPrivateMessage(this.selectedChatUserId, this.newMessageContent.trim());
    this.newMessageContent = '';
  }

  private scrollToBottom(): void {
    if (this.messagesContainer && this.messagesContainer.nativeElement) {
      this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
    }
  }
}
