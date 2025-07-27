import { Injectable, NgZone } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { ChatMessage } from '../Models/chat.models';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection!: signalR.HubConnection;
  private messageSubject = new Subject<ChatMessage>();

  public messages$: Observable<ChatMessage> = this.messageSubject.asObservable();

  constructor(private ngZone: NgZone) { }

  public startConnection = (accessToken: string) => {
    if (!accessToken) {
      console.error("No accessToken provided. Cannot start SignalR connection.");
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7083/chathub', { // تأكد من الـ URL ده
        accessTokenFactory: () => accessToken
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Debug) // لـ Debugging مفصل
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('✅ SignalR Connection started successfully!');
        console.log('Connection State:', this.hubConnection.state);
      })
      .catch(err => console.error('❌ Error while starting SignalR connection: ' + err));

    this.hubConnection.on('ReceiveMessage', (messageData: any) => {
      console.log("🔔 ChatService received message:", messageData);

      this.ngZone.run(() => {
        const receivedMessage: ChatMessage = {
          senderId: messageData.senderId,
          receiverId: messageData.receiverId,
          messageContent: messageData.messageContent, // 🔴 تطابق مع الباك إند
          timeStamp: new Date(messageData.timeStamp) // 🔴 تطابق مع الباك إند
        };

        console.log("📨 Processed message:", receivedMessage);
        this.messageSubject.next(receivedMessage);
        console.log("✅ Message sent to subscribers");
      });
    });

    this.hubConnection.onclose((error) => {
      console.error('❌ SignalR connection closed:', error);
    });

    this.hubConnection.onreconnecting((error) => {
      console.log('🔄 SignalR reconnecting:', error);
    });

    this.hubConnection.onreconnected((connectionId) => {
      console.log('✅ SignalR reconnected:', connectionId);
    });
  }

  // 🔴🔴🔴 هذا الاسم يجب أن يتطابق مع اسم الميثود في الـ Backend ChatHub.cs (SendMessage)
  public sendPrivateMessage = (receiverId: string, messageContent: string) => {
    console.log(`📤 Attempting to send message to ${receiverId}: "${messageContent}"`);
    console.log('Connection State:', this.hubConnection?.state);

    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendMessage', receiverId, messageContent) // 🔴🔴🔴 تأكد أن هذا 'SendMessage' ليتطابق مع الـ Backend
        .then(() => {
          console.log('✅ Message sent successfully via SignalR');
        })
        .catch(err => {
          console.error('❌ Error invoking SendMessage: ' + err);
        });
    } else {
      console.error('❌ SignalR connection not established. Current state:', this.hubConnection?.state);
    }
  }

  public stopConnection = () => {
    if (this.hubConnection && this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      this.hubConnection.stop()
        .then(() => console.log('🛑 SignalR Connection stopped.'))
        .catch(err => console.error('❌ Error while stopping SignalR connection: ' + err));
    }
  }

  public getConnectionState(): string {
    return this.hubConnection?.state || 'Not initialized';
  }
}
