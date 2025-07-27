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

  constructor(private ngZone: NgZone) { } // 🔴 إضافة NgZone

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
      .then(() => {
        console.log('✅ SignalR Connection started successfully!');
        console.log('Connection State:', this.hubConnection.state);
      })
      .catch(err => console.error('❌ Error while starting SignalR connection: ' + err));

    // 🔴 استقبال الرسائل داخل NgZone
    this.hubConnection.on('ReceiveMessage', (messageData: any) => {
      console.log("🔔 ChatService received message:", messageData);

      // 🔴 تشغيل الكود داخل NgZone لضمان تحديث الUI
      this.ngZone.run(() => {
        const receivedMessage: ChatMessage = {
          senderId: messageData.senderId,
          receiverId: messageData.receiverId,
          messageContent: messageData.messageContent,
          timeStamp: new Date(messageData.timeStamp)
        };

        console.log("📨 Processed message:", receivedMessage);
        this.messageSubject.next(receivedMessage);
        console.log("✅ Message sent to subscribers");
      });
    });

    // 🔴 إضافة error handling
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

  public sendPrivateMessage = (receiverId: string, messageContent: string) => {
    console.log(`📤 Attempting to send message to ${receiverId}: "${messageContent}"`);
    console.log('Connection State:', this.hubConnection?.state);

    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendMessage', receiverId, messageContent)
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

  // 🔴 إضافة method للتحقق من حالة الاتصال
  public getConnectionState(): string {
    return this.hubConnection?.state || 'Not initialized';
  }
}
