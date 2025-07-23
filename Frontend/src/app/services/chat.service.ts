// src/app/services/chat.service.ts (ASSUMED CURRENT STATE)

import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';

// Assuming your Message interface is defined somewhere accessible,
// like in a shared `models.ts` or directly in `chat.component.ts`
import { Message } from '../components/chat/chat'; // ✅ Make sure to import your Message interface

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection!: signalR.HubConnection;
  private messageSubject = new Subject<Message>(); // ✅ Changed Subject type to Message

  // Property to expose the messages as an Observable
  public messages$: Observable<Message> = this.messageSubject.asObservable();

  constructor(private http: HttpClient) { }

  public startConnection = () => {
    // Get the token from local storage
    const token = localStorage.getItem('token');
    if (!token) {
      console.error("No token found. Cannot start SignalR connection.");
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7083/chathub', { // Use your actual API URL here
        accessTokenFactory: () => token // Provide the token for authentication
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connection started!'))
      .catch(err => console.error('Error while starting SignalR connection: ' + err));

    // ✅ IMPORTANT: Update the .on method to expect a single 'Message' object
    this.hubConnection.on('ReceiveMessage', (message: Message) => {
      console.log("ChatService received message:", message);
      this.messageSubject.next(message); // Emit the full message object
    });
  }

  // ✅ Updated the signature to expect a single Message object
  public onReceiveMessage = (callback: (message: Message) => void) => {
    this.messageSubject.subscribe(callback);
  }

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
