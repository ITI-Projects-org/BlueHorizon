// src/app/Models/chat.models.ts

export interface ChatMessage {
  id?: string; // ID الرسالة (جعلتها اختيارية لأنها قد لا تكون موجودة دائماً عند الإنشاء)
  senderId: string;
  receiverId: string;
  content: string; // <<== تم التعديل من 'messageContent' إلى 'content'
  timestamp: Date; // <<== تم التعديل من 'timeStamp' إلى 'timestamp' (حرف الـ t صغير)
  // isRead: boolean; // إذا كانت هذه الخاصية موجودة في DTO الرسالة من الـ Backend، أضفها هنا
  // senderUserName?: string; // هذه الخصائص لا تكون جزءاً من الـ Message DTO الأساسي
  // receiverUserName?: string;
  // isMyMessage?: boolean;
  // displaySenderName?: string;
}

export interface InboxItem {
  otherUserId: string; // الـ ID بتاع الطرف التاني في المحادثة
  otherUserName: string; // اسم الطرف التاني
  lastMessageContent: string;
  lastMessageTimestamp: Date; // تأكد من أن الـ Backend يرجعها بـ Format يمكن تحويله لـ Date
  unreadCount: number;
  lastMessageIsRead: boolean; // إذا كنت تتبع قراءة آخر رسالة في الـ Inbox
}

// يمكنك إضافة المزيد من الـ models هنا إذا احتجت
