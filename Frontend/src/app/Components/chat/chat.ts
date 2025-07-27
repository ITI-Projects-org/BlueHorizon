import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { AuthService } from '../../Services/auth.service';
import { Messages } from '../../Services/messages';
import { InboxItem, ChatMessage } from '../../Models/chat.models';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html',
  styleUrls: ['./chat.css']
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {

  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  hubConnection!: HubConnection;
  currentUserId: string | null = null;
  newMessageContent: string = '';

  recentChats: InboxItem[] = [];
  selectedChatUserId: string | null = null;
  selectedChatUser: InboxItem | null = null;
  messages: ChatMessage[] = [];

  private isBrowser: boolean;
  private shouldScrollToBottom: boolean = false;

  constructor(
    private authService: AuthService,
    private messagesService: Messages,
    private route: ActivatedRoute,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.currentUserId = this.authService.getCurrentUserId();
    if (!this.currentUserId && this.isBrowser) {
      console.error('Current user ID is not available. User might not be logged in or token is invalid. Redirecting to login.');
      this.router.navigate(['/login']);
    }
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn() && this.isBrowser) {
      console.log('User not logged in, cannot initialize chat. Redirecting to login.');
      this.router.navigate(['/login']);
      return;
    }

    this.fetchRecentChats();

    if (this.isBrowser) {
      this.startSignalRConnection();
    }

    this.route.queryParams.subscribe(params => {
      const userIdFromUrl = params['userId'];
      if (userIdFromUrl && userIdFromUrl !== this.selectedChatUserId) {
        this.selectChat(userIdFromUrl);
      } else if (!userIdFromUrl && !this.selectedChatUserId && this.recentChats.length > 0) {
        this.selectChat(this.recentChats[0].otherUserId);
      }
    });
  }

  ngAfterViewChecked(): void {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  ngOnDestroy(): void {
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      this.hubConnection.stop().then(() => console.log('SignalR connection stopped.')).catch(err => console.error(err));
    }
  }

  private startSignalRConnection(): void {
    const token = this.authService.getToken();
    if (!token) {
      console.error('No JWT token found for SignalR connection.');
      return;
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7083/chathub', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => {
        console.log('SignalR connection started.');
      })
      .catch(err => console.error('Error while starting SignalR connection: ' + err));

    // 🔴🔴🔴 التغيير الأول: استقبال الكائن كاملاً من الـ Backend
    // الـ Backend يرسل كائناً واحداً، وليس بارامترات منفصلة.
    this.hubConnection.on('ReceiveMessage', (messageData: any) => {
      console.log('Received message from SignalR:', messageData); // للـ Debugging

      const receivedMessage: ChatMessage = {
        senderId: messageData.senderId,
        receiverId: messageData.receiverId,
        // �🔴🔴 التغيير الثاني: استخدام أسماء الخصائص المطابقة من الـ Backend
        // الـ Backend يرسل 'MessageContent' و 'TimeStamp'
        messageContent: messageData.messageContent,
        timeStamp: new Date(messageData.timeStamp)
      };

      // Check if the received message is for the currently selected chat
      if ((this.selectedChatUserId === receivedMessage.senderId && this.currentUserId === receivedMessage.receiverId) ||
          (this.selectedChatUserId === receivedMessage.receiverId && this.currentUserId === receivedMessage.senderId))
      {
          // 🔴🔴🔴 التغيير الثالث: استخدام Spread Operator لتحديث المصفوفة بشكل لا يغيرها في مكانها
          // هذا يضمن أن Angular سيكتشف التغيير ويعيد تحديث الـ UI تلقائياً.
          this.messages = [...this.messages, receivedMessage];
          this.shouldScrollToBottom = true; // Set flag to scroll after view update
      }

      // Always fetch recent chats to update the inbox list (e.g., unread counts, last message preview)
      this.fetchRecentChats();
    });
  }

  // To fetch the list of recent conversations for the Inbox
  fetchRecentChats(): void {
    this.messagesService.getInboxMessages().subscribe({
      next: (inboxItems: InboxItem[]) => {
        this.recentChats = inboxItems;
        if (this.selectedChatUserId) {
            this.selectedChatUser = this.recentChats.find(chat => chat.otherUserId === this.selectedChatUserId) || null;
            // Only load messages if the current chat window is empty or explicitly needs refresh
            // The real-time update logic above handles new messages for active chat
            if (this.messages.length === 0) { // This condition ensures it only loads history once
              this.loadMessagesForSelectedChat();
            }
        } else if (this.recentChats.length > 0) {
            this.selectChat(this.recentChats[0].otherUserId);
        }
      },
      error: (err) => {
        console.error('Error fetching recent chats:', err);
        this.recentChats = []; // Clear list on error
      }
    });
  }

  // When selecting a conversation from the Inbox
  selectChat(otherUserId: string): void {
    this.selectedChatUserId = otherUserId;
    // Find the selected user's data in the Inbox list to update the header
    this.selectedChatUser = this.recentChats.find(chat => chat.otherUserId === otherUserId) || null;
    this.messages = []; // Clear previous messages before loading new ones
    this.newMessageContent = ''; // Clear message input box

    this.loadMessagesForSelectedChat(); // Fetch messages for the selected conversation
  }

  // Method to fetch chat history for the selected user
  private loadMessagesForSelectedChat(): void {
    if (this.selectedChatUserId) {
      this.messagesService.getChatHistory(this.selectedChatUserId).subscribe({
        next: (messages: ChatMessage[]) => {
          this.messages = messages; // Assign retrieved messages to the array displayed in HTML
          this.shouldScrollToBottom = true; // Set flag to scroll after view update
          console.log("The messages from chat.ts:", this.messages); // للـ Debugging
          for (let message of this.messages) {
            console.log(`Message from ${message.senderId} to ${message.receiverId}: ${message.messageContent} at ${message.timeStamp}`);
          }
        },
        error: (err) => {
          console.error('Error fetching conversation messages:', err);
          this.messages = []; // Clear messages on error
        }
      });
    }
  }

  // To send a new message
  sendMessage(): void {
    if (!this.newMessageContent.trim() || !this.selectedChatUserId) {
      return; // Do not send empty message or if no user is selected
    }
    console.log('Sending message:', this.selectedChatUserId, this.newMessageContent.trim()); // للـ Debugging

    this.hubConnection.invoke('SendMessage', this.selectedChatUserId, this.newMessageContent.trim())
      .then(() => {
        this.newMessageContent = ''; // Clear input box after sending
        // الرسالة سيتم إضافتها إلى الـ messages array عن طريق الـ 'ReceiveMessage' event من الـ Hub
        // لذلك لا حاجة لإضافتها هنا يدوياً لتجنب التكرار
      })
      .catch(err => console.error('Error sending message via SignalR: ' + err));
  }

  // Method for auto-scrolling to the bottom of the messages box
  private scrollToBottom(): void {
    if (this.messagesContainer && this.messagesContainer.nativeElement) {
      this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
    }
  }
}

