import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, BehaviorSubject, catchError, throwError } from 'rxjs';
import {
  ChatMessage,
  ChatMessageRequest,
  ChatMessageResponse,
  ChatHistory,
  ChatSuggestion,
} from '../Models/chat.models';

@Injectable({
  providedIn: 'root',
})
export class AIChatService {
  private baseUrl = 'https://localhost:7083/api/Chat'; // Use original port
  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  public messages$ = this.messagesSubject.asObservable();

  constructor(private http: HttpClient) {}

  sendMessage(message: string): Observable<ChatMessageResponse> {
    const request: ChatMessageRequest = { message };
    return this.http.post<ChatMessageResponse>(
      `${this.baseUrl}/message`,
      request
    ).pipe(
      catchError(this.handleError)
    );
  }

  getChatHistory(
    page: number = 1,
    pageSize: number = 50
  ): Observable<ChatHistory> {
    return this.http.get<ChatHistory>(
      `${this.baseUrl}/history?page=${page}&pageSize=${pageSize}`
    ).pipe(
      catchError(this.handleError)
    );
  }

  clearChatHistory(): Observable<any> {
    return this.http.delete(`${this.baseUrl}/clear`).pipe(
      catchError(this.handleError)
    );
  }

  getSuggestions(): Observable<ChatSuggestion> {
    return this.http.get<ChatSuggestion>(`${this.baseUrl}/suggestions`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else if (error.status === 0) {
      // Network error or SSL certificate issue
      errorMessage = 'Unable to connect to server. Please check if the backend is running and SSL certificates are properly configured.';
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    
    console.error('HTTP Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }

  // Local state management
  addMessage(message: ChatMessage): void {
    const currentMessages = this.messagesSubject.value;
    this.messagesSubject.next([...currentMessages, message]);
  }

  addUserMessage(messageText: string): ChatMessage {
    const userMessage: ChatMessage = {
      id: Date.now(), // Temporary ID
      message: messageText,
      response: '',
      createdAt: new Date(),
      isFromUser: true,
    };
    this.addMessage(userMessage);
    return userMessage;
  }

  addBotResponse(response: string): ChatMessage {
    const botMessage: ChatMessage = {
      id: Date.now() + 1, // Temporary ID
      message: '',
      response: response,
      createdAt: new Date(),
      isFromUser: false,
    };
    this.addMessage(botMessage);
    return botMessage;
  }

  loadChatHistory(): void {
    this.getChatHistory().subscribe({
      next: (history) => {
        this.messagesSubject.next(history.messages);
      },
      error: (error) => {
        console.error('Failed to load chat history:', error);
      },
    });
  }

  clearLocalMessages(): void {
    this.messagesSubject.next([]);
  }

  setMessages(messages: ChatMessage[]): void {
    this.messagesSubject.next(messages);
  }

  getMessagesCount(): number {
    return this.messagesSubject.value.length;
  }
}
