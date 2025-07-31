using API.DTOs.ChatDTOs;

namespace API.Services.Interfaces
{
    public interface IAIService
    {
        Task<string> GenerateResponseAsync(string userMessage, string userId, string userRole);
        Task<ChatHistoryDTO> GetChatHistoryAsync(string userId, int page = 1, int pageSize = 50);
        Task<bool> ClearChatHistoryAsync(string userId);
        Task<ChatMessageResponseDTO> SaveChatMessageAsync(string userId, string message, string response);
    }
}
