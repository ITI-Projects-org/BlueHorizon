import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Message } from '../components/chat/chat';

@Injectable({
  providedIn: 'root'
})
export class Messages {
  baseurl: string = 'https://localhost:7083/api/Message';

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    if (token) {
      return new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      });
    }
    return new HttpHeaders({ 'Content-Type': 'application/json' });
  }

  getInboxMessages(): Observable<Message[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<Message[]>(`${this.baseurl}/inbox`, { headers });
  }

  getChatBetweenUsers(otherUserId: string): Observable<Message[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<Message[]>(`${this.baseurl}/chat?otherUserId=${otherUserId}`, { headers });
  }

  sendMessageViaHttp(messageDto: { receiverId: string; messageContent: string }): Observable<any> {
    const headers = this.getAuthHeaders();
    return this.http.post(`${this.baseurl}/SendMessage`, messageDto, { headers });
  }
}
