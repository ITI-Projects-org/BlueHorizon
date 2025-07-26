using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public string Response { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsFromUser { get; set; } = true;
        
        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
