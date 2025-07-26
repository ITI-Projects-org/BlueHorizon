namespace API.DTOs.MessageDTO
{
    public class InboxItemDto
    {
        public string OtherUserId { get; set; } // الـ ID بتاع الشخص التاني في المحادثة
        public string OtherUserName { get; set; } // اسم الشخص التاني
        public string LastMessageContent { get; set; } // محتوى آخر رسالة في المحادثة دي
        public DateTime LastMessageTimestamp { get; set; } // وقت إرسال آخر رسالة
        public bool IsLastMessageFromCurrentUser { get; set; } // هل آخر رسالة دي مرسلة من المستخدم الحالي؟
        public int UnreadMessageCount { get; set; } // عدد الرسائل غير المقروءة في المحادثة دي
                                                    // ممكن تضيف هنا خاصية لـ OtherUserProfilePictureUrl لو عندك صور بروفايل
    }
}
