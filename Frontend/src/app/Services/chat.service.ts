import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { ChatMessage } from '../Models/chat.models'; // تأكد من المسار الصحيح للـ interface

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection!: signalR.HubConnection;
  private messageSubject = new Subject<ChatMessage>(); // نوع الـ Subject هيكون ChatMessage

  public messages$: Observable<ChatMessage> = this.messageSubject.asObservable();

  constructor() { } // مش محتاج HttpClient هنا لو استخدامك مقتصر على SignalR

  public startConnection = () => {
    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) {
      console.error("No accessToken found. Cannot start SignalR connection.");
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7083/chathub', {
        accessTokenFactory: () => accessToken
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connection started!'))
      .catch(err => console.error('Error while starting SignalR connection: ' + err));

    // استقبال الرسائل من الـ Hub
    this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
      console.log("ChatService received message:", message);
      this.messageSubject.next(message);
    });
  }

  // ميثود للإشتراك في الرسائل الجديدة
  public onReceiveMessage = (callback: (message: ChatMessage) => void) => {
    this.messageSubject.subscribe(callback);
  }

  // ميثود لإرسال رسالة خاصة لمستخدم معين
  public sendPrivateMessage = (receiverId: string, messageContent: string) => {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendMessageToUser', receiverId, messageContent)
        .catch(err => console.error('Error invoking SendMessageToUser: ' + err));
    } else {
      console.error('SignalR connection not established. Cannot send message.');
    }
  }

  public stopConnection = () => {
    if (this.hubConnection && this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      this.hubConnection.stop()
        .then(() => console.log('SignalR Connection stopped.'))
        .catch(err => console.error('Error while stopping SignalR connection: ' + err));
    }
  }
}
