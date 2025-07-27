// src/app/Models/chat.models.ts

export interface ChatMessage {
  id?: string; // Message ID (made optional as it might not always exist on creation)
  senderId: string;
  receiverId: string;
  messageContent: string; // <<== Corrected from 'messageContent' to 'content'
  timeStamp: Date; // <<== Corrected from 'timeStamp' to 'timestamp' (lowercase 't')
  // isRead: boolean; // If this property exists in the message DTO from the Backend, add it here
  // senderUserName?: string; // These properties are not part of the basic Message DTO
  // receiverUserName?: string;
  // isMyMessage?: boolean;
  // displaySenderName?: string;
}

export interface InboxItem {
  otherUserId: string; // The ID of the other party in the conversation
  otherUserName: string; // The name of the other party
  lastMessageContent: string;
  lastMessageTimestamp: Date; // Ensure Backend returns it in a format that can be converted to Date
  unreadCount: number;
  lastMessageIsRead: boolean; // If you track the read status of the last message in the Inbox
}

// You can add more models here if needed
