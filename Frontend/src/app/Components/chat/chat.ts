import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common'; // لـ ngIf, ngFor, date pipe
import { ActivatedRoute, Router } from '@angular/router'; // إضافة Router للتنقل
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { AuthService } from '../../Services/auth.service';
import { Messages } from '../../Services/messages';
import { InboxItem, ChatMessage } from '../../Models/chat.models'; // استخدام ChatMessage
import { FormsModule } from '@angular/forms'; // لـ [(ngModel)]

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule], // تأكد من استيراد هذه الـ Modules
  templateUrl: './chat.html',
  styleUrls: ['./chat.css']
})
export class ChatComponent implements OnInit, OnDestroy {

  @ViewChild('messagesContainer') messagesContainer!: ElementRef; // لعمل scroll تلقائي للأسفل

  hubConnection!: HubConnection; // SignalR Hub connection
  currentUserId: string | null = null; // الـ ID بتاع المستخدم الحالي
  newMessageContent: string = ''; // محتوى الرسالة الجديدة اللي بيكتبها المستخدم

  recentChats: InboxItem[] = []; // قائمة المحادثات في الـ Inbox (الجانب الأيسر)
  selectedChatUserId: string | null = null; // الـ ID بتاع المستخدم اللي مختارينه للشات
  selectedChatUser: InboxItem | null = null; // بيانات المستخدم اللي مختارينه (عشان اسمه يظهر في الـ header)
  messages: ChatMessage[] = []; // الرسائل اللي بتظهر في الشات الحالي مع المستخدم المختار

  constructor(
    private authService: AuthService,
    private messagesService: Messages,
    private route: ActivatedRoute,
    private router: Router // حقن الـ Router
  ) {
    this.currentUserId = this.authService.getCurrentUserId(); // جلب الـ ID بتاع المستخدم الحالي
    if (!this.currentUserId) {
      console.error('Current user ID is not available. User might not be logged in or token is invalid.');
      // يمكنك توجيه المستخدم لصفحة تسجيل الدخول هنا لو الـ ID مش موجود
      this.router.navigate(['/login']);
    }
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      console.log('User not logged in, cannot initialize chat. Redirecting to login.');
      this.router.navigate(['/login']);
      return;
    }

    // جلب الرسائل الأخيرة في الـ Inbox عند بدء المكون
    this.fetchRecentChats();

    // تشغيل الـ SignalR connection
    this.startSignalRConnection();

    // الاشتراك في تغييرات الـ queryParams
    // ده بيستخدم لو فيه رابط بيوديك على صفحة الشات مع userId معين (مثلاً من صفحة الـ Units)
    this.route.queryParams.subscribe(params => {
      const userIdFromUrl = params['userId'];
      if (userIdFromUrl && userIdFromUrl !== this.selectedChatUserId) {
        this.selectChat(userIdFromUrl);
      }
    });
  }

  ngOnDestroy(): void {
    // إيقاف الـ SignalR connection عند تدمير المكون لتجنب الـ memory leaks
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      this.hubConnection.stop().then(() => console.log('SignalR connection stopped.')).catch(err => console.error(err));
    }
  }

  private startSignalRConnection(): void {
    const token = this.authService.getToken(); // جلب الـ JWT Token من AuthService
    if (!token) {
      console.error('No JWT token found for SignalR connection.');
      // التعامل مع عدم وجود Token (مثل إعادة توجيه المستخدم لتسجيل الدخول)
      return;
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7083/chathub', { // تأكد من أن هذا الـ URL صحيح ومطابق لـ Backend
        accessTokenFactory: () => token // تمرير الـ token للـ SignalR
      })
      .withAutomaticReconnect() // إعادة الاتصال تلقائياً لو انقطع الاتصال
      .build();

    this.hubConnection.start()
      .then(() => {
        console.log('SignalR connection started.');
      })
      .catch(err => console.error('Error while starting SignalR connection: ' + err));

    // استقبال الرسائل من الـ SignalR Hub
    this.hubConnection.on('ReceiveMessage', (senderId: string, receiverId: string, content: string, timestamp: string) => {
      const receivedMessage: ChatMessage = {
        senderId: senderId,
        receiverId: receiverId,
        content: content,
        timestamp: new Date(timestamp) // تحويل الـ string timestamp إلى Date object
      };

      // إضافة الرسالة إلى قائمة الرسائل المعروضة إذا كانت تخص الشات المختار حالياً
      // الرسالة ممكن تكون مرسلة من المستخدم المختار إليّ، أو أنا أرسلتها للمستخدم المختار
      if ((this.selectedChatUserId === receivedMessage.senderId && this.currentUserId === receivedMessage.receiverId) ||
          (this.selectedChatUserId === receivedMessage.receiverId && this.currentUserId === receivedMessage.senderId))
      {
          this.messages.push(receivedMessage);
          this.scrollToBottom(); // التمرير للأسفل بعد إضافة الرسالة
      }

      // تحديث قائمة الـ Inbox لإظهار آخر رسالة أو عدد الرسائل غير المقروءة
      // يمكن تحسين هذا الجزء لتحديث العنصر المحدد فقط بدلاً من جلب كل شيء
      this.fetchRecentChats();
    });
  }

  // لجلب قائمة المحادثات الأخيرة للـ Inbox
  fetchRecentChats(): void {
    this.messagesService.getInboxMessages().subscribe({
      next: (inboxItems: InboxItem[]) => {
        this.recentChats = inboxItems;
        // إذا كان هناك selectedChatUserId بالفعل، قم بتحديث بياناته
        if (this.selectedChatUserId) {
            this.selectedChatUser = this.recentChats.find(chat => chat.otherUserId === this.selectedChatUserId) || null;
            // إذا كانت قائمة الرسائل الحالية فارغة (مثلاً أول مرة نفتح شات معين)، قم بتحميلها
            if (this.messages.length === 0) {
              this.loadMessagesForSelectedChat();
            }
        } else if (this.recentChats.length > 0) {
            // إذا لم يتم اختيار أي شات، اختر أول شات في القائمة تلقائياً
            this.selectChat(this.recentChats[0].otherUserId);
        }
      },
      error: (err) => {
        console.error('Error fetching recent chats:', err);
        this.recentChats = []; // مسح القائمة في حالة الخطأ
      }
    });
  }

  // عند اختيار محادثة من الـ Inbox
  selectChat(otherUserId: string): void {
    this.selectedChatUserId = otherUserId;
    // ابحث عن بيانات المستخدم المختار في قائمة الـ Inbox لتحديث الـ header
    this.selectedChatUser = this.recentChats.find(chat => chat.otherUserId === otherUserId) || null;
    this.messages = []; // مسح الرسائل السابقة قبل تحميل الرسائل الجديدة
    this.newMessageContent = ''; // مسح مربع كتابة الرسالة

    this.loadMessagesForSelectedChat(); // جلب الرسائل الخاصة بالمحادثة المختارة
  }

  // ميثود لجلب سجل المحادثة للمستخدم المختار
  private loadMessagesForSelectedChat(): void {
    if (this.selectedChatUserId) {
      this.messagesService.getChatHistory(this.selectedChatUserId).subscribe({
        next: (messages: ChatMessage[]) => {
          this.messages = messages; // تعيين الرسائل المسترجعة للـ array اللي بتظهر في الـ HTML
          this.scrollToBottom(); // التمرير للأسفل بعد تحميل الرسائل
        },
        error: (err) => {
          console.error('Error fetching conversation messages:', err);
          this.messages = []; // مسح الرسائل في حالة الخطأ
        }
      });
    }
  }

  // لإرسال رسالة جديدة
  sendMessage(): void {
    if (!this.newMessageContent.trim() || !this.selectedChatUserId) {
      return; // لا ترسل رسالة فارغة أو إذا لم يتم اختيار مستخدم
    }

    // إرسال الرسالة عبر SignalR Hub (باستخدام الميثود SendMessage في الـ Hub)
    this.hubConnection.invoke('SendMessage', this.selectedChatUserId, this.newMessageContent.trim())
      .then(() => {
        this.newMessageContent = ''; // مسح مربع الكتابة بعد الإرسال
        // الرسالة سيتم إضافتها إلى الـ messages array عن طريق الـ 'ReceiveMessage' event من الـ Hub
      })
      .catch(err => console.error('Error sending message via SignalR: ' + err));
  }

  // ميثود للتمرير التلقائي لأسفل صندوق الرسائل
  private scrollToBottom(): void {
    // استخدام setTimeout لضمان أن الـ DOM قد تم تحديثه قبل محاولة التمرير
    setTimeout(() => {
      if (this.messagesContainer && this.messagesContainer.nativeElement) {
        this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
      }
    }, 100); // تأخير بسيط (مثلاً 100 ملي ثانية) يعطي الوقت الكافي للـ DOM للتحديث
  }
}
