// src/app/components/chat/chat.ts

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { JwtHelperService, JWT_OPTIONS } from '@auth0/angular-jwt';
import { ChatService } from '../../services/chat.service';

// ✅ استخدام الـ Interface اللي أنت عرفتها
export interface Message {
  id?: number; // أو string لو الـ ID في الـ Backend بتاعك string
  senderId: string;
  receiverId: string;
  messageContent: string;
  timestamp?: string; // أو Date لو عايز تحولها لـ Date object في الـ Frontend
  isRead?: boolean; // ✅ ممكن تضيفها لو الرسالة فيها خاصية الـ IsRead
}

@Component({
  selector: 'app-chat',
  standalone: true,
  templateUrl: './chat.html',
  styleUrls: ['./chat.css'],
  imports: [CommonModule, FormsModule],
  providers: [
    { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
    JwtHelperService
  ]
})
export class Chat implements OnInit {
  // ✅ استخدام الـ Message interface كنوع للـ messages array
  messages: { from: string; content: string }[] = []; // سيبها زي ما هي عشان متكسرش الـ UI الحالي
                                                    // وممكن مستقبلاً تخليها messages: Message[] = [];
  messageText: string = '';
  toUserId: string = '';
  currentUserId: string = '';

  constructor(
    private chatService: ChatService,
    private jwtHelper: JwtHelperService
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      const decoded: any = this.jwtHelper.decodeToken(token);
      this.currentUserId = decoded?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

      console.log("Current User ID:", this.currentUserId);

      this.toUserId = '5c246abe-1e3f-4913-9e33-4380cbb2667c';

      this.chatService.startConnection();

      // ✅ التعديل هنا: استقبال الـ object من نوع Message interface
      this.chatService.onReceiveMessage((messageObject: Message) => { // ✅ استقبال object من نوع Message
        console.log("Received message object from Hub:", messageObject);

        // ✅ إضافة الرسالة للـ array بتاعك باستخدام الخصائص من الـ object المستلم
        this.messages.push({
          from: messageObject.senderId,
          content: messageObject.messageContent
        });
      });
    } else {
      console.warn("No token found. Chat connection not started.");
    }
  }

  sendMessage(): void {
    if (!this.toUserId || !this.messageText.trim()) {
        console.warn("Cannot send message: Receiver ID or message text is empty.");
        return;
    }

    this.chatService.sendPrivateMessage(this.toUserId, this.messageText);
    this.messages.push({ from: 'You', content: this.messageText });
    this.messageText = '';
    console.log("Message sent from UI to service:", { to: this.toUserId, message: this.messageText });
  }
}
