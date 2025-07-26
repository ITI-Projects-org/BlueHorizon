import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service'; // تأكد من المسار الصحيح لـ AuthService
import { InboxItem, ChatMessage } from '../Models/chat.models'; // استخدام ChatMessage

@Injectable({
  providedIn: 'root'
})
export class Messages {
  // تأكد أن هذا الـ URL يطابق الـ URL الأساسي للـ API في الـ Backend
  baseurl: string = 'https://localhost:7083/api/Message';

  constructor(private http: HttpClient, private authService: AuthService) { }

  private getAuthHeaders(): HttpHeaders {
    // هذه الميثود تجلب الـ Access Token من AuthService وتضيفه للـ Headers
    const accessToken = this.authService.getToken(); // استخدم getAccessToken() أو getToken() حسب اسم الميثود في AuthService
    if (!accessToken) {
      console.error('No access token found for API request.');
      // يمكنك هنا إعادة توجيه المستخدم لصفحة تسجيل الدخول أو التعامل مع الخطأ
      return new HttpHeaders({ 'Content-Type': 'application/json' });
    }
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${accessToken}`
    });
  }

  // لجلب قائمة المحادثات الأخيرة (الـ Inbox)
  getInboxMessages(): Observable<InboxItem[]> {
    const headers = this.getAuthHeaders();
    console.log('Fetching inbox messages from API:', `${this.baseurl}/inbox`);
    return this.http.get<InboxItem[]>(`${this.baseurl}/inbox`, { headers });
  }

  // لجلب سجل المحادثة بين المستخدمين
  // **التعديل هنا:** تم تغيير الـ URL لإرسال 'otherUserId' كـ Query Parameter
  getChatHistory(otherUserId: string): Observable<ChatMessage[]> {
    const headers = this.getAuthHeaders();
    const url = `${this.baseurl}/history?otherUserId=${otherUserId}`; // <--- التغيير هنا
    console.log(`Fetching chat history with ${otherUserId} from API:`, url);
    return this.http.get<ChatMessage[]>(url, { headers });
  }

  // إذا كنت تستخدم HTTP Post لإرسال الرسائل (بالإضافة إلى SignalR أو كـ fallback)
  // يمكنك تفعيل هذه الميثود
  // sendMessageHttp(receiverId: string, messageContent: string): Observable<ChatMessage> {
  //   const headers = this.getAuthHeaders();
  //   const body = { receiverId, messageContent }; // تأكد من مطابقة اسم الـ Property في الـ DTO الخاص بـ SendMessage
  //   return this.http.post<ChatMessage>(`${this.baseurl}/SendMessage`, body, { headers });
  // }
}
