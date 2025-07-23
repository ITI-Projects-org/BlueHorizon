import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from './auth.service'; // تأكد إن المسار ده صح لملف الـ AuthService بتاعك

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection!: signalR.HubConnection;

  constructor(private authService: AuthService) {}

  public startConnection(): void {
    // 1. استخراج الـ JWT Token من الـ AuthService
    const token = this.authService.getToken();
    if (!token) {
      console.error("Authentication token not found. Cannot start SignalR connection.");
      return; // وقف الاتصال لو مفيش توكن
    }

    // 2. بناء الـ HubConnection
    // - تأكد إن الـ URL (https://localhost:7083/chathub) هو نفسه اللي الباك إند شغال عليه (HTTP/HTTPS والـ Port)
    // - التوكن بيتبعت كـ Query Parameter بإسم 'access_token'
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`https://localhost:7083/chathub?access_token=${token}`, {
        // إذا كنت بتستخدم الـ token فقط في الـ Query String، فـ `withCredentials: true` قد لا تكون ضرورية
        // ولكن إضافتها ما بتضرش لو فيه Cookies أو Auth Headers تانية محتاج تبعتها
        // withCredentials: true
      })
      .withAutomaticReconnect() // إعادة الاتصال تلقائياً عند فقدان الاتصال
      .build();

    // 3. بدء الاتصال والتعامل مع النتائج (نجاح أو فشل)
    this.hubConnection
      .start()
      .then(() => {
        console.log('✅ SignalR Connected');
        // هنا ممكن تضيف أي منطق تاني بعد الاتصال بنجاح، زي الانضمام لغرفة دردشة معينة
      })
      .catch(err => {
        console.error('❌ SignalR Error: Failed to start connection', err);
        // ممكن تعرض رسالة للمستخدم أو تحاول تبدأ الاتصال تاني بعد فترة
      });
  }

  /**
   * دالة للاشتراك في رسائل الاستقبال من السيرفر.
   * السيرفر هينادي "ReceiveMessage" وهيبعت user و message.
   * @param callback الدالة اللي هتتنفذ لما تيجي رسالة جديدة.
   */
  public onReceiveMessage(callback: (user: string, message: string) => void): void {
    // بتستقبل رسائل اسمها 'ReceiveMessage' من الـ Hub
    this.hubConnection.on('ReceiveMessage', callback);
  }

  /**
   * دالة لإرسال رسالة خاصة لمستخدم معين.
   * بتنادي الدالة "SendMessageToUser" في الـ Hub على السيرفر.
   * @param toUserId الـ ID بتاع المستخدم المستهدف.
   * @param message نص الرسالة.
   */
  public sendPrivateMessage(toUserId: string, message: string): void {
    // لازم اسم الدالة يكون متطابق تماماً مع اسم الدالة في الـ Hub بالباك إند (SendMessageToUser)
    this.hubConnection.invoke('SendMessageToUser', toUserId, message)
      .catch(err => console.error('❌ Send Error: Failed to send message', err));
  }

  // ممكن تضيف دوال تانية هنا للتعامل مع أحداث SignalR زي onclose, onreconnecting, onreconnected
  public onConnectionClosed(callback: (error?: Error) => void): void {
    this.hubConnection.onclose(callback);
  }
}
