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
        public async Task SendMessage_EmptyMessage_ReturnsBadRequest()
        {
            // Arrange
            var request = new ChatMessageRequestDTO { Message = "" };
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Role, "tenant")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
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
            var request = new ChatMessageRequestDTO { Message = "   " };
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Role, "tenant")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Act
            var result = await _controller.SendMessage(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task SendMessage_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var request = new ChatMessageRequestDTO { Message = "Test message" };
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await _controller.SendMessage(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task GetChatHistory_VerifiesServiceCall()
        {
            // Arrange
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            var mockHistory = new ChatHistoryDTO
            {
                Messages = new List<ChatMessageResponseDTO>(),
                TotalCount = 0
            };

            _mockAIService.Setup(s => s.GetChatHistoryAsync("test-user-id", 1, 50))
                        .ReturnsAsync(mockHistory);

            // Act
            await _controller.GetChatHistory(1, 50);

            // Assert
            _mockAIService.Verify(s => s.GetChatHistoryAsync("test-user-id", 1, 50), Times.Once);
        }

        [TestMethod]
        public async Task GetChatHistory_ReturnsOkResult()
        {
            // Arrange
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            var mockHistory = new ChatHistoryDTO
            {
                Messages = new List<ChatMessageResponseDTO>
                {
                    new ChatMessageResponseDTO { Id = 1, Message = "Hello", Response = "Hi there!" }
                },
                TotalCount = 1
            };

            _mockAIService.Setup(s => s.GetChatHistoryAsync("test-user-id", 1, 50))
                        .ReturnsAsync(mockHistory);

            // Act
            var result = await _controller.GetChatHistory(1, 50);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ChatHistoryDTO));
        }

        [TestMethod]
        public async Task GetChatHistory_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await _controller.GetChatHistory(1, 50);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }
    }
}