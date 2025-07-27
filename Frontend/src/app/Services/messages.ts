import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service'; // Ensure correct path to AuthService
import { InboxItem, ChatMessage } from '../Models/chat.models'; // Use ChatMessage

@Injectable({
  providedIn: 'root'
})
export class Messages {
  // Ensure this URL matches your Backend API base URL
  baseurl: string = 'https://localhost:7083/api/Message';

  constructor(private http: HttpClient, private authService: AuthService) { }

  private getAuthHeaders(): HttpHeaders {
    // This method fetches the Access Token from AuthService and adds it to the Headers
    const accessToken = this.authService.getToken(); // Use getAccessToken() or getToken() based on your AuthService method name
    if (!accessToken) {
      console.error('No access token found for API request.');
      // You can redirect the user to the login page here or handle the error
      return new HttpHeaders({ 'Content-Type': 'application/json' });
    }
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${accessToken}`
    });
  }

  // To fetch the list of recent conversations (Inbox)
  getInboxMessages(): Observable<InboxItem[]> {
    const headers = this.getAuthHeaders();
    console.log('Fetching inbox messages from API:', `${this.baseurl}/inbox`);
    var x = this.http.get<InboxItem[]>(`${this.baseurl}/inbox`, { headers });
    console.log('Inbox messages fetched:', x);
    return this.http.get<InboxItem[]>(`${this.baseurl}/inbox`, { headers });
  }

  // To fetch chat history between users
  // **Modification here:** The URL was changed to send 'otherUserId' as a Query Parameter
  getChatHistory(otherUserId: string): Observable<ChatMessage[]> {
    const headers = this.getAuthHeaders();
    const url = `${this.baseurl}/history?otherUserId=${otherUserId}`; // <--- Change is here
    console.log(`Fetching chat history with ${otherUserId} from API:`, url);
    var y = this.http.get<ChatMessage[]>(url, { headers });
    console.log('Chat history fetched:', y);
    return this.http.get<ChatMessage[]>(url, { headers });
  }

  // If you use HTTP Post to send messages (in addition to SignalR or as a fallback)
  // You can enable this method
  // sendMessageHttp(receiverId: string, messageContent: string): Observable<ChatMessage> {
  //   const headers = this.getAuthHeaders();
  //   const body = { receiverId, messageContent }; // Ensure property name matches your SendMessage DTO
  //   return this.http.post<ChatMessage>(`${this.baseurl}/SendMessage`, body, { headers });
  // }
}
