namespace API.DTOs.ChatDTOs
{
    public class ChatMessageRequestDTO
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ChatMessageResponseDTO
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsFromUser { get; set; }
    }

    public class ChatHistoryDTO
    {
        public List<ChatMessageResponseDTO> Messages { get; set; } = new List<ChatMessageResponseDTO>();
        public int TotalCount { get; set; }
    }
}
