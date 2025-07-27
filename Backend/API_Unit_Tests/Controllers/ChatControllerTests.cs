using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using API.Controllers;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.ChatDTOs;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace API_Unit_Tests.Controllers
{
    [TestClass]
    public class ChatControllerTests : BaseTestClass
    {
        private ChatController _controller = null!;
        private Mock<IAIService> _mockAIService = null!;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _mockAIService = new Mock<IAIService>();
            _controller = new ChatController(_mockAIService.Object);
        }

        [TestMethod]
        public async Task SendMessage_ValidMessage_ReturnsOkResult()
        {
            // Arrange
            var request = new ChatMessageRequestDTO
            {
                Message = "Hello, how are you?"
            };

            var expectedResponse = new ChatMessageResponseDTO
            {
                Id = 1,
                Message = request.Message,
                Response = "I'm doing well, thank you!",
                CreatedAt = DateTime.UtcNow,
                IsFromUser = true
            };

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Role, "Tenant")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            _mockAIService.Setup(s => s.GenerateResponseAsync(request.Message, "user123", "Tenant"))
                         .ReturnsAsync("I'm doing well, thank you!");
            _mockAIService.Setup(s => s.SaveChatMessageAsync("user123", request.Message, "I'm doing well, thank you!"))
                         .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.SendMessage(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            // The result is an anonymous object with success, message, and response properties
            Assert.IsNotNull(okResult.Value);
            var resultValue = okResult.Value;
            Assert.IsNotNull(resultValue);
            
            // Check if the result has the expected structure using reflection
            var resultType = resultValue.GetType();
            var successProperty = resultType.GetProperty("success");
            var messageProperty = resultType.GetProperty("message");
            var responseProperty = resultType.GetProperty("response");
            
            Assert.IsNotNull(successProperty);
            Assert.IsNotNull(messageProperty);
            Assert.IsNotNull(responseProperty);
            
            var successValue = successProperty.GetValue(resultValue);
            Assert.IsNotNull(successValue);
            Assert.IsTrue((bool)successValue);
        }

        [TestMethod]
        public async Task SendMessage_EmptyMessage_ReturnsBadRequest()
        {
            // Arrange
            var request = new ChatMessageRequestDTO
            {
                Message = ""
            };

            // Act
            var result = await _controller.SendMessage(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task SendMessage_WhitespaceMessage_ReturnsBadRequest()
        {
            // Arrange
            var request = new ChatMessageRequestDTO
            {
                Message = "   "
            };

            // Act
            var result = await _controller.SendMessage(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetChatHistory_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            var expectedHistory = new ChatHistoryDTO
            {
                Messages = new List<ChatMessageResponseDTO>
                {
                    new ChatMessageResponseDTO { Id = 1, Message = "Hello", Response = "Hi there!", CreatedAt = DateTime.UtcNow, IsFromUser = true }
                },
                TotalCount = 1
            };

            _mockAIService.Setup(s => s.GetChatHistoryAsync("user123", 1, 50))
                         .ReturnsAsync(expectedHistory);

            // Act
            var result = await _controller.GetChatHistory(1, 50);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ChatHistoryDTO));
        }

        [TestMethod]
        public async Task ClearChatHistory_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            _mockAIService.Setup(s => s.ClearChatHistoryAsync("user123"))
                         .ReturnsAsync(true);

            // Act
            var result = await _controller.ClearChatHistory();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task ClearChatHistory_ServiceReturnsFalse_ReturnsInternalServerError()
        {
            // Arrange
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            _mockAIService.Setup(s => s.ClearChatHistoryAsync("user123"))
                         .ReturnsAsync(false);

            // Act
            var result = await _controller.ClearChatHistory();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult!.StatusCode);
        }

        [TestMethod]
        public async Task TestGemini_ValidMessage_ReturnsOkResult()
        {
            // Arrange
            string testMessage = "Test message for Gemini";
            _mockAIService.Setup(s => s.GenerateResponseAsync(testMessage, It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync("Gemini response");

            // Act
            var result = await _controller.TestGemini(testMessage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void TestConfig_ReturnsOkResult()
        {
            // Act
            var result = _controller.TestConfig();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up resources if needed
        }
    }
}
