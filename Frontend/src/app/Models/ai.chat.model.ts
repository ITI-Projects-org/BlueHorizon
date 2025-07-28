export interface ChatMessageRequest {
  message: string;
}

export interface ChatMessageResponse {
  success: boolean;
  message: ChatMessage;
  response: string;
}

export interface ChatHistory {
  messages: ChatMessage[];
  totalCount: number;
}

export interface ChatSuggestion {
  suggestions: string[];
}

export interface ChatMessage {
  id: number;
  message: string;
  response: string;
  createdAt: Date;
  isFromUser: boolean;
}
