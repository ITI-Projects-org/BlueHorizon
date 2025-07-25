import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { JwtHelperService, JWT_OPTIONS } from '@auth0/angular-jwt';
import { ChatService } from '../../services/chat.service';
import { Messages } from '../../services/messages';

export interface Message {
  id?: number;
  senderId: string;
  receiverId: string;
  messageContent: string;
  timestamp?: string;
  isRead?: boolean;
  from: string;
  content: string;
  isMyMessage?: boolean;
}

@Component({
  selector: 'app-chat',
  standalone: true,
  templateUrl: './chat.html',
  styleUrls: ['./chat.css'],
  imports: [CommonModule, FormsModule, DatePipe],
  providers: [
    { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
    JwtHelperService
  ]
})
export class Chat implements OnInit, OnDestroy {
  messages: Message[] = [];
  messageText: string = '';
  toUserId: string = '';
  currentUserId: string = '';

  constructor(
    private chatService: ChatService,
    private jwtHelper: JwtHelperService,
    private messagesService: Messages
  ) {}

  ngOnDestroy(): void {
    this.chatService.stopConnection();
    console.log("Chat component destroyed. SignalR connection stopped.");
  }

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      const decoded: any = this.jwtHelper.decodeToken(token);
      console.log("Decoded token:", decoded);

      this.currentUserId = decoded?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

      console.log("Current User ID:", this.currentUserId);

      this.chatService.startConnection();

      this.chatService.onReceiveMessage((messageObject: Message) => {
        console.log("Received new real-time message object from Hub:", messageObject);

        this.messages.push({
          ...messageObject,
          from: messageObject.senderId === this.currentUserId ? 'You' : 'User',
          content: messageObject.messageContent,
          isMyMessage: messageObject.senderId === this.currentUserId
        });
        console.log("Updated messages array with new real-time message:", this.messages);
      });

    } else {
      console.warn("No token found. Chat connection not started.");
    }
  }

  onToUserIdChange(): void {
    if (this.currentUserId && this.toUserId && this.toUserId.trim() !== '') {
      this.messagesService.getChatBetweenUsers(this.toUserId).subscribe({
        next: (historicalMessages: Message[]) => {
          this.messages = historicalMessages.map(msg => ({
            ...msg,
            from: msg.senderId === this.currentUserId ? 'You' : 'User',
            content: msg.messageContent,
            isMyMessage: msg.senderId === this.currentUserId
          }));
          console.log("Historical messages loaded for new chat:", this.messages);
        },
        error: (err) => {
          console.error("Error loading historical messages for new chat:", err);
        }
      });
    } else {
      this.messages = []; 
    }
  }


  sendMessage(): void {
    if (!this.toUserId || !this.messageText.trim()) {
        console.warn("Cannot send message: Receiver ID or message text is empty.");
        return;
    }

    this.chatService.sendPrivateMessage(this.toUserId, this.messageText);

    this.messages.push({
      senderId: this.currentUserId,
      receiverId: this.toUserId,
      messageContent: this.messageText,
      from: 'You',
      content: this.messageText,
      isMyMessage: true,
      timestamp: new Date().toISOString()
    });
    this.messageText = '';
    console.log("Message sent from UI to service:", { to: this.toUserId, message: this.messageText });
  }
}
