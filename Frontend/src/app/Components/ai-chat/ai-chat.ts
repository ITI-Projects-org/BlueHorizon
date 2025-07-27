import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  ElementRef,
  AfterViewChecked,
} from '@angular/core';
import { ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AIChatService } from '../../Services/ai-chat.service';
import { ChatMessage } from '../../Models/ai.chat.model';
import { Subscription } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-ai-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ai-chat.html',
  styleUrl: './ai-chat.css',
})
export class AIChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  @ViewChild('messageInput') private messageInput!: ElementRef;

  messages: ChatMessage[] = [];
  currentMessage: string = '';
  isLoading: boolean = false;
  isChatOpen: boolean = false;
  suggestions: string[] = [];
  showSuggestions: boolean = true;

  private subscription: Subscription = new Subscription();
  private shouldScrollToBottom: boolean = false;
  constructor(
    private aiChatService: AIChatService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // Subscribe to messages
    this.subscription.add(
      this.aiChatService.messages$.subscribe((messages) => {
        this.messages = messages;
        this.shouldScrollToBottom = true;
      })
    );

    // Only load chat data when chat is opened to avoid unnecessary API calls
    // This prevents errors from causing page refresh loops
  }

  ngAfterViewChecked(): void {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  toggleChat(): void {
    this.isChatOpen = !this.isChatOpen;
    if (this.isChatOpen) {
      // Load data only when chat is opened
      if (this.messages.length === 0) {
        this.loadChatHistory();
      }
      if (this.suggestions.length === 0) {
        this.loadSuggestions();
      }

      this.cdr.detectChanges(); // Force UI update when opening chat

      setTimeout(() => {
        this.messageInput?.nativeElement?.focus();
      }, 100);
    }
  }

  sendMessage(): void {
    if (!this.currentMessage.trim() || this.isLoading) {
      return;
    }

    const messageText = this.currentMessage.trim();
    this.currentMessage = '';
    this.showSuggestions = false;
    this.isLoading = true;

    // Add user message to UI immediately
    this.aiChatService.addUserMessage(messageText);
    this.cdr.detectChanges(); // Force UI update

    // Send to backend
    this.aiChatService.sendMessage(messageText).subscribe({
      next: (response) => {
        if (response.success) {
          this.aiChatService.addBotResponse(response.response);
        } else {
          this.aiChatService.addBotResponse(
            'Sorry, I encountered an error. Please try again.'
          );
        }
        this.isLoading = false;
        this.cdr.detectChanges(); // Force UI update to show bot response
      },
      error: (error) => {
        console.error('Error sending message:', error);
        this.aiChatService.addBotResponse(
          "Sorry, I'm having trouble connecting. Please try again later."
        );
        this.isLoading = false;
        this.cdr.detectChanges(); // Force UI update even on error
      },
    });
  }

  sendSuggestion(suggestion: string): void {
    this.currentMessage = suggestion;
    this.cdr.detectChanges(); // Update input field
    this.sendMessage();
  }

  loadChatHistory(): void {
    // Gracefully handle chat history loading
    this.aiChatService.getChatHistory().subscribe({
      next: (history) => {
        // Convert backend chat messages to separate user and bot messages for UI
        const uiMessages: ChatMessage[] = [];

        history.messages.forEach((chatMessage) => {
          // Add user message
          if (chatMessage.message && chatMessage.message.trim()) {
            uiMessages.push({
              id: chatMessage.id * 2, // Ensure unique IDs
              message: chatMessage.message,
              response: '',
              createdAt: chatMessage.createdAt,
              isFromUser: true,
            });
          }

          // Add bot response
          if (chatMessage.response && chatMessage.response.trim()) {
            uiMessages.push({
              id: chatMessage.id * 2 + 1, // Ensure unique IDs
              message: '',
              response: chatMessage.response,
              createdAt: chatMessage.createdAt,
              isFromUser: false,
            });
          }
        });

        this.aiChatService.setMessages(uiMessages);
        this.cdr.detectChanges(); // Force UI update
      },
      error: (error) => {
        console.warn('Chat history unavailable:', error.message);
        // Don't show error to user, just continue without history
      },
    });
  }

  loadSuggestions(): void {
    this.aiChatService.getSuggestions().subscribe({
      next: (response) => {
        this.suggestions = response.suggestions;
        this.cdr.detectChanges(); // Force UI update
      },
      error: (error) => {
        console.warn('Suggestions unavailable:', error.message);
        // Set default suggestions if API fails
        this.suggestions = [
          'How does BlueHorizon work?',
          'Tell me about booking process',
          'What are the platform fees?',
          'How to contact support?',
          'Platform safety measures',
        ];
        this.cdr.detectChanges(); // Force UI update even with default suggestions
      },
    });
  }

  clearChat(): void {
    Swal.fire({
      title: 'Caution',
      text: 'Are you sure you want to delete the chat?',
      icon: 'question',
      draggable: true,
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel',
    }).then((result) => {
      if (result.isConfirmed) {
        this.aiChatService.clearChatHistory().subscribe({
          next: (response) => {
            if (response.success) {
              this.aiChatService.clearLocalMessages();
              this.showSuggestions = true;
              // Reload chat history and suggestions after clearing
              this.loadChatHistory();
              this.loadSuggestions();
              this.cdr.detectChanges();
            }
          },
          error: (error) => {
            console.error('Error clearing chat:', error);
          },
        });
      }
    });
  }

  onInputKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  private scrollToBottom(): void {
    try {
      if (this.messagesContainer) {
        const element = this.messagesContainer.nativeElement;
        element.scrollTop = element.scrollHeight;
      }
    } catch (err) {
      console.error('Error scrolling to bottom:', err);
    }
  }

  formatTime(date: Date): string {
    return new Date(date).toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  isToday(date: Date): boolean {
    const today = new Date();
    const messageDate = new Date(date);
    return messageDate.toDateString() === today.toDateString();
  }

  formatDate(date: Date): string {
    const messageDate = new Date(date);
    if (this.isToday(messageDate)) {
      return 'Today';
    }
    return messageDate.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
    });
  }

  shouldShowDateSeparator(index: number): boolean {
    if (index === 0) return true;

    const currentMessage = this.messages[index];
    const previousMessage = this.messages[index - 1];

    const currentDate = new Date(currentMessage.createdAt).toDateString();
    const previousDate = new Date(previousMessage.createdAt).toDateString();

    return currentDate !== previousDate;
  }

  trackMessage(index: number, message: ChatMessage): any {
    return message.id || index;
  }

  formatBotMessage(message: string): string {
    if (!message) return '';

    // Convert line breaks to <br> tags
    let formatted = message.replace(/\n/g, '<br>');

    // Convert **bold** to <strong>
    formatted = formatted.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');

    // Convert *italic* to <em>
    formatted = formatted.replace(/\*(.*?)\*/g, '<em>$1</em>');

    // Convert `code` to <code>
    formatted = formatted.replace(/`(.*?)`/g, '<code>$1</code>');

    return formatted;
  }
}
