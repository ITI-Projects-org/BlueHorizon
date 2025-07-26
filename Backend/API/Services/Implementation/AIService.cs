using API.DTOs.ChatDTOs;
using API.Models;
using API.Services.Interfaces;
using API.UnitOfWorks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Text.Json;

namespace API.Services.Implementation
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _openAIApiKey;

        public AIService(
            HttpClient httpClient,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _openAIApiKey = _configuration["OpenAI:apiKey"] ?? throw new ArgumentNullException("OpenAI API key not found");
            
            // Set up HttpClient for OpenAI API
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAIApiKey}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "BlueHorizon-AI-Bot/1.0");
        }

        public async Task<string> GenerateResponseAsync(string userMessage, string userId, string userRole)
        {
            try
            {
                // Check if message is related to the platform
                if (!IsMessageRelatedToPlatform(userMessage))
                {
                    return "I'm BlueHorizon's AI assistant and I can only help you with questions related to our vacation rental platform. Please ask me about bookings, properties, payments, or any other platform-related topics.";
                }

                var systemPrompt = GetSystemPrompt(userRole, userId);
                var contextualPrompt = await GetContextualPromptAsync(userMessage, userId, userRole);

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = $"{contextualPrompt}\n\nUser question: {userMessage}" }
                    },
                    max_tokens = 500,
                    temperature = 0.7
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    // Log the detailed error for debugging
                    Console.WriteLine($"OpenAI API Error: {response.StatusCode} - {response.ReasonPhrase}");
                    Console.WriteLine($"Error Content: {errorContent}");
                    Console.WriteLine($"API Key (first 10 chars): {_openAIApiKey?.Substring(0, Math.Min(10, _openAIApiKey.Length))}...");
                    
                    return "I apologize, but I'm having trouble processing your request right now. Please try again later.";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);

                return openAIResponse?.choices?.FirstOrDefault()?.message?.content ?? 
                       "I apologize, but I couldn't generate a proper response. Please try again.";
            }
            catch (Exception ex)
            {
                // Log the exception with more details
                Console.WriteLine($"Exception in AIService: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return "I'm experiencing technical difficulties. Please try again later.";
            }
        }

        public async Task<ChatHistoryDTO> GetChatHistoryAsync(string userId, int page = 1, int pageSize = 50)
        {
            var allMessages = await _unitOfWork.ChatMessageRepository.GetAllAsync();
            var userMessages = allMessages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(m => m.CreatedAt)
                .ToList();

            var messageDTOs = _mapper.Map<List<ChatMessageResponseDTO>>(userMessages);

            return new ChatHistoryDTO
            {
                Messages = messageDTOs,
                TotalCount = allMessages.Count(m => m.UserId == userId)
            };
        }

        public async Task<bool> ClearChatHistoryAsync(string userId)
        {
            try
            {
                var allMessages = await _unitOfWork.ChatMessageRepository.GetAllAsync();
                var userMessages = allMessages.Where(m => m.UserId == userId).ToList();

                foreach (var message in userMessages)
                {
                    await _unitOfWork.ChatMessageRepository.DeleteByIdAsync(message.Id);
                }

                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ChatMessageResponseDTO> SaveChatMessageAsync(string userId, string message, string response)
        {
            var chatMessage = new ChatMessage
            {
                UserId = userId,
                Message = message,
                Response = response,
                CreatedAt = DateTime.UtcNow,
                IsFromUser = true
            };

            await _unitOfWork.ChatMessageRepository.AddAsync(chatMessage);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ChatMessageResponseDTO>(chatMessage);
        }

        private bool IsMessageRelatedToPlatform(string message)
        {
            var platformKeywords = new[]
            {
                "booking", "reservation", "unit", "property", "vacation", "rental", 
                "payment", "owner", "tenant", "review", "amenity", "price", "availability",
                "check-in", "check-out", "guest", "host", "bluehorizon", "platform",
                "accommodation", "stay", "village", "sea", "bedroom", "bathroom",
                "cancel", "refund", "profile", "account", "help", "support"
            };

            var lowerMessage = message.ToLower();
            return platformKeywords.Any(keyword => lowerMessage.Contains(keyword)) ||
                   lowerMessage.Contains("blue horizon") ||
                   lowerMessage.Length < 10; // Allow short greetings
        }

        private string GetSystemPrompt(string userRole, string userId)
        {
            var basePrompt = @"You are BlueHorizon's AI assistant, a helpful chatbot for a vacation rental platform similar to Airbnb. 
            You should be friendly, professional, and knowledgeable about vacation rentals, bookings, and property management.
            
            Platform features include:
            - Property listings and bookings
            - Owner and tenant accounts
            - Payment processing
            - Review systems
            - Property management tools
            - QR code access systems
            
            Always provide helpful, accurate information related to vacation rentals and the platform.";

            return userRole.ToLower() switch
            {
                "tenant" => basePrompt + @"
                
                The user is a TENANT (guest). Focus on helping them with:
                - Finding and booking properties
                - Understanding booking process and policies
                - Payment and refund information
                - Check-in/check-out procedures
                - Contacting property owners
                - Leaving reviews
                - Managing their bookings",

                "owner" => basePrompt + @"
                
                The user is an OWNER (host). Focus on helping them with:
                - Property listing optimization
                - Pricing strategies and revenue maximization
                - Guest communication best practices
                - Property management tips
                - Understanding booking analytics
                - Managing amenities and descriptions
                - Handling guest reviews
                - Platform policies for hosts",

                "admin" => basePrompt + @"
                
                The user is an ADMIN. Focus on helping them with:
                - Platform management and oversight
                - User account management
                - Booking dispute resolution
                - System analytics and reporting
                - Platform policies and procedures",

                _ => basePrompt + " The user role is not specified, provide general platform assistance."
            };
        }

        private async Task<string> GetContextualPromptAsync(string userMessage, string userId, string userRole)
        {
            var contextBuilder = new StringBuilder();

            try
            {
                if (userRole.ToLower() == "tenant")
                {
                    // Get user's recent bookings for context
                    var bookings = await _unitOfWork.BookingRepository.GetAllAsync();
                    var userBookings = bookings.Where(b => b.TenantId == userId).Take(3).ToList();
                    
                    if (userBookings.Any())
                    {
                        contextBuilder.AppendLine("User's recent bookings context:");
                        foreach (var booking in userBookings)
                        {
                            contextBuilder.AppendLine($"- Booking {booking.Id}: {booking.CheckInDate:MMM dd} to {booking.CheckOutDate:MMM dd}, Status: {booking.PaymentStatus}");
                        }
                    }
                }
                else if (userRole.ToLower() == "owner")
                {
                    // Get owner's properties for context
                    var units = await _unitOfWork.UnitRepository.GetAllAsync();
                    var userUnits = units.Where(u => u.OwnerId == userId).Take(3).ToList();
                    
                    if (userUnits.Any())
                    {
                        contextBuilder.AppendLine("Owner's properties context:");
                        foreach (var unit in userUnits)
                        {
                            contextBuilder.AppendLine($"- {unit.Title}: {unit.UnitType}, ${unit.BasePricePerNight}/night");
                        }
                    }

                    // Add popular amenities and pricing insights
                    contextBuilder.AppendLine("\nPlatform insights for owners:");
                    contextBuilder.AppendLine("- Properties with sea access typically earn 20-30% more");
                    contextBuilder.AppendLine("- High-demand amenities: WiFi, Kitchen, Parking, AC");
                    contextBuilder.AppendLine("- Peak seasons vary by location - consider dynamic pricing");
                }
            }
            catch
            {
                // If context gathering fails, continue without it
            }

            return contextBuilder.ToString();
        }

        // Helper class for OpenAI response deserialization
        private class OpenAIResponse
        {
            public Choice[]? choices { get; set; }
        }

        private class Choice
        {
            public Message? message { get; set; }
        }

        private class Message
        {
            public string? content { get; set; }
        }
    }
}
