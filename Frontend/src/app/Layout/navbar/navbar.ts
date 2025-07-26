import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common'; // لـ ngIf, ngFor, date pipe
import { Router, RouterModule } from '@angular/router'; // إضافة Router و RouterModule
import { AuthService } from '../../Services/auth.service';
import { Messages } from '../../Services/messages';
import { InboxItem } from '../../Models/chat.models';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'; // لـ SignalR

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule], // تأكد من استيراد RouterModule هنا
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class NavbarComponent implements OnInit, OnDestroy {
  isScrolled: boolean = false;
  showPopup: boolean = false; // للتحكم في ظهور الـ message popup
  recentChats: InboxItem[] = []; // لعرض آخر المحادثات في الـ popup
  unreadMessagesCount: number = 0; // لعدد الرسائل غير المقروءة في الـ badge
  hubConnection!: HubConnection; // SignalR connection for real-time updates

  constructor(
    private router: Router,
    private authService: AuthService,
    private messagesService: Messages // حقن الـ Messages Service
  ) { }

  ngOnInit(): void {
    window.addEventListener('scroll', this.onWindowScroll.bind(this));

    // جلب الرسائل الأخيرة في الـ Navbar Popup عند تحميل الصفحة
    if (this.authService.isLoggedIn()) {
      this.fetchRecentChatsForPopup();
      this.startSignalRConnectionForNavbar(); // بدء اتصال SignalR للـ Navbar
    }
  }

  ngOnDestroy(): void {
    window.removeEventListener('scroll', this.onWindowScroll.bind(this));
    // إيقاف الـ SignalR connection عند تدمير المكون
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      this.hubConnection.stop().then(() => console.log('Navbar SignalR connection stopped.')).catch(err => console.error(err));
    }
  }

  onWindowScroll(): void {
    this.isScrolled = window.scrollY > 50; // غير القيمة حسب ارتفاع الـ hero section
  }

  toggleMessagePopup(): void {
    this.showPopup = !this.showPopup;
    // لما الـ popup يظهر، ممكن تجيب آخر تحديث للرسائل (اختياري)
    if (this.showPopup && this.authService.isLoggedIn()) {
      this.fetchRecentChatsForPopup();
    }
  }

  // ميثود لجلب الرسائل الأخيرة للـ Navbar popup
  fetchRecentChatsForPopup(): void {
    this.messagesService.getInboxMessages().subscribe({
      next: (inboxItems: InboxItem[]) => {
        this.recentChats = inboxItems.slice(0, 5); // عرض آخر 5 محادثات مثلاً
        this.unreadMessagesCount = inboxItems.reduce((sum, chat) => sum + chat.unreadCount, 0);
      },
      error: (err) => {
        console.error('Error fetching recent chats for navbar:', err);
        this.recentChats = [];
        this.unreadMessagesCount = 0;
      }
    });
  }

  // للانتقال إلى صفحة الشات من الـ popup
  goToChat(otherUserId: string): void {
    this.router.navigate(['/chat'], { queryParams: { userId: otherUserId } });
    this.toggleMessagePopup(); // إخفاء الـ popup بعد الضغط
  }

  // بدء اتصال SignalR للـ Navbar فقط لتحديث الـ Inbox
  private startSignalRConnectionForNavbar(): void {
    const token = this.authService.getToken();
    if (!token) {
      console.error('No JWT token found for Navbar SignalR connection.');
      return;
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7083/chathub', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => {
        console.log('Navbar SignalR connection started.');
      })
      .catch(err => console.error('Error while starting Navbar SignalR connection: ' + err));

    // استقبال الرسائل لتحديث الـ Navbar Inbox
    this.hubConnection.on('ReceiveMessage', () => {
      // عند استقبال أي رسالة، أعد جلب قائمة الـ Inbox لتحديثها
      this.fetchRecentChatsForPopup();
    });
  }

  // ... (باقي ميثودات الـ Navbar زي isLoggedin و getCurrentUserName و logout) ...
  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  getCurrentUserName(): string | null {
    return this.authService.getCurrentUserName(); // تأكد من اسم الميثود في AuthService
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']); // أو أي صفحة بعد الـ logout
  }
}
