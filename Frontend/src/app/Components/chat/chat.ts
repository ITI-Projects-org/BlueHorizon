import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked, Inject, PLATFORM_ID, NgZone, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { Messages } from '../../Services/messages'; // خدمة HTTP لجلب الـ Inbox والـ History
import { ChatService } from '../../Services/chat.service'; // 🔴🔴🔴 استيراد خدمة الشات الجديدة
import { InboxItem, ChatMessage } from '../../Models/chat.models';
import { Subscription } from 'rxjs'; // لإدارة الاشتراكات

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
  private messageSubscription!: Subscription; // للاشتراك في رسائل SignalR

  constructor(
    private authService: AuthService,
    private messagesService: Messages,
    private chatService: ChatService, // 🔴🔴🔴 حقن خدمة الشات
    private route: ActivatedRoute,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object,
    private ngZone: NgZone, // لضمان تحديث الـ UI
    private cdr: ChangeDetectorRef // لفرض تحديث الـ UI
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

    // 🔴🔴🔴 بدء اتصال SignalR عبر الخدمة
    if (this.isBrowser) {
      const token = this.authService.getToken();
      if (token) {
        this.chatService.startConnection(token);
      } else {
        console.error('JWT Token not found for ChatService connection. Redirecting to login.');
        this.router.navigate(['/login']);
      }
    }

    // 🔴🔴🔴 الاشتراك في رسائل الـ Real-time من ChatService
    this.messageSubscription = this.chatService.messages$.subscribe(
      (message: ChatMessage) => {
        // يتم تشغيل هذا الـ Callback داخل Angular Zone بواسطة NgZone
        // لأننا نستخدم NgZone.run() في ChatService عند next()
        // ولكن للتأكد التام من تحديث الـ UI، سنستخدم ngZone.run() و cdr.detectChanges() هنا أيضاً
        this.ngZone.run(() => {
          // إضافة الرسالة فقط إذا كانت تخص المحادثة المحددة حالياً
          if ((this.selectedChatUserId === message.senderId && this.currentUserId === message.receiverId) ||
              (this.selectedChatUserId === message.receiverId && this.currentUserId === message.senderId))
          {
              this.messages = [...this.messages, message]; // تحديث المصفوفة بشكل لا يغيرها في مكانها
              this.shouldScrollToBottom = true;
          }
          this.fetchRecentChats(); // تحديث قائمة المحادثات لتعكس الرسائل الجديدة (مثل عدد غير المقروء)
          this.cdr.detectChanges(); // فرض تحديث الـ UI
        });
      }
    );

    // 🔴🔴🔴 التعامل مع الـ userId من الـ queryParams عند تهيئة الـ Component
    this.route.queryParams.subscribe(params => {
      const userIdFromUrl = params['userId'];
      if (userIdFromUrl) {
        this.selectedChatUserId = userIdFromUrl; // تعيين الـ userId من الـ URL
      }
      this.fetchRecentChats(); // جلب المحادثات بعد تحديد الـ userId
    });
  }

  ngAfterViewChecked(): void {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  ngOnDestroy(): void {
    // 🔴🔴🔴 إيقاف اتصال SignalR عبر الخدمة
    if (this.isBrowser) {
      this.chatService.stopConnection();
    }
    // 🔴🔴🔴 إلغاء الاشتراك لمنع تسرب الذاكرة
    if (this.messageSubscription) {
      this.messageSubscription.unsubscribe();
    }
  }

  fetchRecentChats(): void {
    this.messagesService.getInboxMessages().subscribe({
      next: (inboxItems: InboxItem[]) => {
        this.ngZone.run(() => { // التأكد من تشغيل التحديث داخل Angular Zone
          this.recentChats = inboxItems;

          // 🔴🔴🔴 منطق تحديد الشات عند التحميل أو عند جلب الـ Inbox
          if (this.selectedChatUserId) {
            // إذا كان هناك userId محدد (من الـ URL أو من اختيار سابق)
            this.selectedChatUser = this.recentChats.find(chat => chat.otherUserId === this.selectedChatUserId) || null;

            if (!this.selectedChatUser) {
              // 🔴🔴🔴 إذا لم يتم العثور على المستخدم في الـ Inbox (محادثة جديدة)
              // قم بإنشاء InboxItem مؤقت لعرض اسمه في الـ Header
              this.selectedChatUser = {
                otherUserId: this.selectedChatUserId,
                otherUserName: `User: ${this.selectedChatUserId.substring(0, 8)}...`, // اسم مؤقت
                lastMessageContent: '',
                lastMessageTimestamp: new Date(),
                unreadCount: 0,
                lastMessageIsRead: true
              };
              // يمكنك هنا إضافة هذا الكائن المؤقت إلى this.recentChats إذا أردت أن يظهر في القائمة فوراً
              // this.recentChats.unshift(this.selectedChatUser);
            }

            // تحميل الرسائل الخاصة بالمحادثة المحددة (سواء كانت جديدة أو موجودة)
            this.loadMessagesForSelectedChat();
          } else if (this.recentChats.length > 0) {
            // إذا لم يتم تحديد أي محادثة، اختر أول محادثة في القائمة تلقائياً
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
    this.selectedChatUserId = otherUserId;
    this.selectedChatUser = this.recentChats.find(chat => chat.otherUserId === otherUserId) || null;

    if (!this.selectedChatUser) {
      // 🔴🔴🔴 إذا لم يتم العثور على المستخدم (محادثة جديدة)، قم بإنشاء InboxItem مؤقت
      this.selectedChatUser = {
        otherUserId: otherUserId,
        otherUserName: `User: ${otherUserId.substring(0, 8)}...`, // اسم مؤقت
        lastMessageContent: '',
        lastMessageTimestamp: new Date(),
        unreadCount: 0,
        lastMessageIsRead: true
      };
    }

    this.messages = []; // مسح الرسائل السابقة
    this.newMessageContent = ''; // مسح مربع الكتابة

    this.loadMessagesForSelectedChat();
  }

  private loadMessagesForSelectedChat(): void {
    if (this.selectedChatUserId) {
      this.messagesService.getChatHistory(this.selectedChatUserId).subscribe({
        next: (messages: ChatMessage[]) => {
          this.ngZone.run(() => {
            this.messages = messages;
            this.shouldScrollToBottom = true;
            console.log("The messages from chat.ts:", this.messages);
            for (let message of this.messages) {
              console.log(`Message from ${message.senderId} to ${message.receiverId}: ${message.messageContent} at ${message.timeStamp}`);
            }
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
      return;
    }
    console.log('Sending message to:', this.selectedChatUserId, 'Content:', this.newMessageContent.trim());

    this.chatService.sendPrivateMessage(this.selectedChatUserId, this.newMessageContent.trim());
    this.newMessageContent = '';
  }

  private scrollToBottom(): void {
    if (this.messagesContainer && this.messagesContainer.nativeElement) {
      this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
    }
  }
}
