import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';

import { Message } from '../components/chat/chat';
@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection!: signalR.HubConnection;
  private messageSubject = new Subject<Message>();

  public messages$: Observable<Message> = this.messageSubject.asObservable();

  constructor(private http: HttpClient) { }

  public startConnection = () => {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error("No token found. Cannot start SignalR connection.");
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:7083/chathub', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connection started!'))
      .catch(err => console.error('Error while starting SignalR connection: ' + err));

    this.hubConnection.on('ReceiveMessage', (message: Message) => {
      console.log("ChatService received message:", message);
      this.messageSubject.next(message);
    });
  }

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
