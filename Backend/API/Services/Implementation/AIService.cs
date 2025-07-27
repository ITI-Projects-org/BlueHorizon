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
        // Commented out OpenAI configuration
        // private readonly string _openAIApiKey;
        private readonly string _geminiApiKey;
        private static DateTime _lastApiCall = DateTime.MinValue;
        private static readonly object _rateLimitLock = new object();

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
            
            // Commented out OpenAI configuration
            // _openAIApiKey = _configuration["OpenAI:apiKey"] ?? throw new ArgumentNullException("OpenAI API key not found");
            
            // Configure for Gemini API
            _geminiApiKey = _configuration["Gemini:apiKey"] ?? throw new ArgumentNullException("Gemini API key not found");
            
            // Set up HttpClient for Gemini API
            _httpClient.DefaultRequestHeaders.Clear();
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

                // Simple rate limiting to avoid hitting API too frequently
                lock (_rateLimitLock)
                {
                    var timeSinceLastCall = DateTime.UtcNow - _lastApiCall;
                    var minInterval = TimeSpan.FromSeconds(2); // Minimum 2 seconds between calls
                    
                    if (timeSinceLastCall < minInterval)
                    {
                        var delayNeeded = minInterval - timeSinceLastCall;
                        Thread.Sleep(delayNeeded);
                    }
                    
                    _lastApiCall = DateTime.UtcNow;
                }

                var systemPrompt = GetSystemPrompt(userRole, userId);
                var contextualPrompt = await GetContextualPromptAsync(userMessage, userId, userRole);

                // Commented out OpenAI request
                /*
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
                */

                // Gemini API request body - using gemini-1.5-flash for better free tier limits
                var requestBody = new
                {
                    contents = new[]
                    {
                        new 
                        {
                            parts = new[]
                            {
                                new { text = $"{systemPrompt}\n\n{contextualPrompt}\n\nUser question: {userMessage}" }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 32,
                        topP = 0.9,
                        maxOutputTokens = 500, // Reduced for better rate limiting and faster responses
                        stopSequences = new string[] { }
                    },
                    safetySettings = new[]
                    {
                        new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Use Gemini Flash model - better for free tier limits
                var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_geminiApiKey}", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    // Handle specific error types
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        Console.WriteLine($"Gemini API Rate Limit Exceeded: {response.StatusCode}");
                        Console.WriteLine($"Error Content: {errorContent}");
                        return "I'm currently experiencing high demand. Please wait a moment and try again, or ask a simpler question to help reduce processing time.";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        Console.WriteLine($"Gemini API Quota Exceeded: {response.StatusCode}");
                        return "I've reached my daily usage limit. Please try again tomorrow or contact support for assistance.";
                    }
                    else
                    {
                        // Log the detailed error for debugging
                        Console.WriteLine($"Gemini API Error: {response.StatusCode} - {response.ReasonPhrase}");
                        Console.WriteLine($"Error Content: {errorContent}");
                        Console.WriteLine($"API Key (first 10 chars): {_geminiApiKey?.Substring(0, Math.Min(10, _geminiApiKey.Length))}...");
                        return "I apologize, but I'm having trouble processing your request right now. Please try again later.";
                    }
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);

                return geminiResponse?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text ?? 
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
            // Shortened prompt to reduce token usage
            var basePrompt = @"You are BlueHorizon's AI assistant for a vacation rental platform. Be friendly, professional, and concise.

Platform features: Property listings, bookings, payments, reviews, QR access.
Keep responses brief and helpful.";

            return userRole.ToLower() switch
            {
                "tenant" => basePrompt + " User is a GUEST. Help with: bookings, payments, check-in/out, reviews.",
                "owner" => basePrompt + " User is a HOST. Help with: listings, pricing, guests, property management.",
                "admin" => basePrompt + " User is an ADMIN. Help with: platform management, analytics, disputes.",
                _ => basePrompt + " Provide general platform assistance."
            };
        }

        private async Task<string> GetContextualPromptAsync(string userMessage, string userId, string userRole)
        {
            // Simplified context to reduce token usage
            var contextBuilder = new StringBuilder();

            try
            {
                if (userRole.ToLower() == "tenant")
                {
                    var bookings = await _unitOfWork.BookingRepository.GetAllAsync();
                    var userBookings = bookings.Where(b => b.TenantId == userId).Take(2).ToList();
                    
                    if (userBookings.Any())
                    {
                        contextBuilder.AppendLine("Recent bookings:");
                        foreach (var booking in userBookings)
                        {
                            contextBuilder.AppendLine($"- {booking.CheckInDate:MMM dd}-{booking.CheckOutDate:MMM dd}, Status: {booking.PaymentStatus}");
                        }
                    }
                }
                else if (userRole.ToLower() == "owner")
                {
                    var units = await _unitOfWork.UnitRepository.GetAllAsync();
                    var userUnits = units.Where(u => u.OwnerId == userId).Take(2).ToList();
                    
                    if (userUnits.Any())
                    {
                        contextBuilder.AppendLine("Your properties:");
                        foreach (var unit in userUnits)
                        {
                            contextBuilder.AppendLine($"- {unit.Title}: ${unit.BasePricePerNight}/night");
                        }
                    }
                }
            }
            catch
            {
                // If context gathering fails, continue without it
            }

            return contextBuilder.ToString();
        }

        // Helper classes for OpenAI response deserialization (commented out)
        /*
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
        */

        // Helper classes for Gemini response deserialization
        private class GeminiResponse
        {
            public GeminiCandidate[]? candidates { get; set; }
        }

        private class GeminiCandidate
        {
            public GeminiContent? content { get; set; }
        }

        private class GeminiContent
        {
            public GeminiPart[]? parts { get; set; }
        }

        private class GeminiPart
        {
            public string? text { get; set; }
        }
    }
}
