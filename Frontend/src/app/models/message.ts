export interface Message {
  id?: number;
  senderId: string;
  receiverId: string;
  messageContent: string;
  timestamp?: string;
}
