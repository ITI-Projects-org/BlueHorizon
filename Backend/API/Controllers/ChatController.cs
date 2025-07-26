using API.DTOs.ChatDTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IAIService _aiService;

        public ChatController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { msg = "Message cannot be empty" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "tenant";

            try
            {
                // Generate AI response
                var aiResponse = await _aiService.GenerateResponseAsync(request.Message, userId, userRole);

                // Save chat message
                var savedMessage = await _aiService.SaveChatMessageAsync(userId, request.Message, aiResponse);

                return Ok(new
                {
                    success = true,
                    message = savedMessage,
                    response = aiResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    msg = "An error occurred while processing your message. Please try again." 
                });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var chatHistory = await _aiService.GetChatHistoryAsync(userId, page, pageSize);
                return Ok(chatHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Failed to retrieve chat history" });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearChatHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var success = await _aiService.ClearChatHistoryAsync(userId);
                if (success)
                {
                    return Ok(new { success = true, msg = "Chat history cleared successfully" });
                }
                else
                {
                    return StatusCode(500, new { success = false, msg = "Failed to clear chat history" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, msg = "An error occurred while clearing chat history" });
            }
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "tenant";

            var suggestions = userRole.ToLower() switch
            {
                "tenant" => new[]
                {
                    "Show me available properties near the sea",
                    "How do I cancel my booking?",
                    "What amenities are most popular?",
                    "Help me find pet-friendly accommodations",
                    "What's the refund policy?"
                },
                "owner" => new[]
                {
                    "How can I optimize my property pricing?",
                    "What amenities should I add to increase bookings?",
                    "Tips for writing better property descriptions",
                    "How to handle difficult guests?",
                    "Best practices for property photos"
                },
                "admin" => new[]
                {
                    "Show platform analytics overview",
                    "How to handle booking disputes?",
                    "User account management guidelines",
                    "Platform policy updates",
                    "Revenue optimization strategies"
                },
                _ => new[]
                {
                    "How does BlueHorizon work?",
                    "Tell me about booking process",
                    "What are the platform fees?",
                    "How to contact support?",
                    "Platform safety measures"
                }
            };

            return Ok(new { suggestions });
        }

        // Test endpoint for OpenAI integration (remove in production)
        [HttpPost("test-openai")]
        [AllowAnonymous]
        public async Task<IActionResult> TestOpenAI([FromBody] string message)
        {
            try
            {
                var response = await _aiService.GenerateResponseAsync(message, "test-user", "tenant");
                return Ok(new { success = true, response = response });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.Message });
            }
        }

        // Simple endpoint to check OpenAI API key configuration
        [HttpGet("test-config")]
        [AllowAnonymous]
        public IActionResult TestConfig()
        {
            try
            {
                var config = Request.HttpContext.RequestServices.GetService<IConfiguration>();
                var apiKey = config["OpenAI:apiKey"];
                
                return Ok(new { 
                    hasApiKey = !string.IsNullOrEmpty(apiKey),
                    apiKeyPrefix = apiKey?.Substring(0, Math.Min(10, apiKey?.Length ?? 0)) + "...",
                    apiKeyLength = apiKey?.Length ?? 0
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.Message });
            }
        }
    }
}
