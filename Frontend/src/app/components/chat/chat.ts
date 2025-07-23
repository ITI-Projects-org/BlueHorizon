import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { JwtHelperService, JWT_OPTIONS } from '@auth0/angular-jwt';
import { ChatService } from '../../services/chat.service';

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
  messages: { from: string; content: string }[] = [];
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
      const decoded = this.jwtHelper.decodeToken(token);
      this.currentUserId = decoded?.['nameidentifier'];
      this.toUserId = '5c246abe-1e3f-4913-9e33-4380cbb2667c'; // مثال ثابت مؤقت

      this.chatService.startConnection();
      this.chatService.onReceiveMessage((from, msg) => {
        this.messages.push({ from, content: msg });
      });
    } else {
    }
  }

  sendMessage(): void {
    if (!this.toUserId || !this.messageText.trim()) return;

    this.chatService.sendPrivateMessage(this.toUserId, this.messageText);
    this.messages.push({ from: 'You', content: this.messageText });
    this.messageText = '';
  }
}
