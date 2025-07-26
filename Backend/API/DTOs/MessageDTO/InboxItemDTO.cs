namespace API.DTOs.MessageDTO
{
    public class InboxItemDto
    {
        public string OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string LastMessageContent { get; set; }
        public DateTime LastMessageTimestamp { get; set; }
        public bool IsLastMessageFromCurrentUser { get; set; }
        public int UnreadMessageCount { get; set; }
    }
}
