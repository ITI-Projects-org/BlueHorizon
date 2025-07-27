using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using API.Controllers;
using API.DTOs.MessageDTO;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace API_Unit_Tests.Controllers
{
    [TestClass]
    public class MessageControllerTests : BaseTestClass
    {
        private MessageController _controller = null!;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            _controller = new MessageController(MockMapper.Object, MockUnitOfWork.Object);

            // Setup controller context for claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Role, "Tenant")
            };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [TestMethod]
        public async Task GetInbox_HasMessages_ReturnsOkResult()
        {
            // Arrange
            var userId = "test-user-id";
            var messages = new List<Message>
            {
                new Message
                {
                    Id = 1,
                    SenderId = "sender1",
                    ReceiverId = userId,
                    MessageContent = "Hello",
                    TimeStamp = DateTime.Now
                },
                new Message
                {
                    Id = 2,
                    SenderId = "sender2",
                    ReceiverId = userId,
                    MessageContent = "Hi there",
                    TimeStamp = DateTime.Now
                }
            };

            var messageDTOs = new List<MessageDto>
            {
                new MessageDto
                {
                    SenderId = "sender1",
                    ReceiverId = userId,
                    MessageContent = "Hello",
                    SenderUsername = "Sender1",
                    ReceiverUsername = "TestUser"
                },
                new MessageDto
                {
                    SenderId = "sender2",
                    ReceiverId = userId,
                    MessageContent = "Hi there",
                    SenderUsername = "Sender2",
                    ReceiverUsername = "TestUser"
                }
            };

            MockUnitOfWork.Setup(u => u.MessageRepository.GetInboxAsync(userId))
                         .ReturnsAsync(messages);
            MockMapper.Setup(m => m.Map<List<MessageDto>>(messages))
                     .Returns(messageDTOs);

            // Act
            var result = await _controller.GetInbox();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedMessages = okResult.Value as List<MessageDto>;
            Assert.IsNotNull(returnedMessages);
            Assert.AreEqual(2, returnedMessages.Count);
            Assert.AreEqual("Hello", returnedMessages[0].MessageContent);
        }

        [TestMethod]
        public async Task GetInbox_NoMessages_ReturnsEmptyList()
        {
            // Arrange
            var userId = "test-user-id";
            var messages = new List<Message>();
            var messageDTOs = new List<MessageDto>();

            MockUnitOfWork.Setup(u => u.MessageRepository.GetInboxAsync(userId))
                         .ReturnsAsync(messages);
            MockMapper.Setup(m => m.Map<List<MessageDto>>(messages))
                     .Returns(messageDTOs);

            // Act
            var result = await _controller.GetInbox();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedMessages = okResult.Value as List<MessageDto>;
            Assert.IsNotNull(returnedMessages);
            Assert.AreEqual(0, returnedMessages.Count);
        }

        [TestMethod]
        public async Task GetChat_ValidOtherUserId_ReturnsOkResult()
        {
            // Arrange
            var currentUserId = "test-user-id";
            var otherUserId = "other-user-id";
            
            var messages = new List<Message>
            {
                new Message
                {
                    Id = 1,
                    SenderId = currentUserId,
                    ReceiverId = otherUserId,
                    MessageContent = "Hello",
                    TimeStamp = DateTime.Now
                },
                new Message
                {
                    Id = 2,
                    SenderId = otherUserId,
                    ReceiverId = currentUserId,
                    MessageContent = "Hi back",
                    TimeStamp = DateTime.Now.AddMinutes(1)
                }
            };

            var messageDTOs = new List<MessageDto>
            {
                new MessageDto
                {
                    SenderId = currentUserId,
                    ReceiverId = otherUserId,
                    MessageContent = "Hello",
                    SenderUsername = "TestUser",
                    ReceiverUsername = "OtherUser"
                },
                new MessageDto
                {
                    SenderId = otherUserId,
                    ReceiverId = currentUserId,
                    MessageContent = "Hi back",
                    SenderUsername = "OtherUser",
                    ReceiverUsername = "TestUser"
                }
            };

            MockUnitOfWork.Setup(u => u.MessageRepository.GetChatBetweenUsersAsync(currentUserId, otherUserId))
                         .ReturnsAsync(messages);
            MockMapper.Setup(m => m.Map<List<MessageDto>>(messages))
                     .Returns(messageDTOs);

            // Act
            var result = await _controller.GetChat(otherUserId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedMessages = okResult.Value as List<MessageDto>;
            Assert.IsNotNull(returnedMessages);
            Assert.AreEqual(2, returnedMessages.Count);
        }

        [TestMethod]
        public async Task GetChat_NoMessages_ReturnsEmptyList()
        {
            // Arrange
            var currentUserId = "test-user-id";
            var otherUserId = "other-user-id";
            var messages = new List<Message>();
            var messageDTOs = new List<MessageDto>();

            MockUnitOfWork.Setup(u => u.MessageRepository.GetChatBetweenUsersAsync(currentUserId, otherUserId))
                         .ReturnsAsync(messages);
            MockMapper.Setup(m => m.Map<List<MessageDto>>(messages))
                     .Returns(messageDTOs);

            // Act
            var result = await _controller.GetChat(otherUserId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            
            var returnedMessages = okResult.Value as List<MessageDto>;
            Assert.IsNotNull(returnedMessages);
            Assert.AreEqual(0, returnedMessages.Count);
        }

        [TestMethod]
        public async Task SendMessage_ValidMessage_ReturnsOkResult()
        {
            // Arrange
            var sendMessageDto = new SendMessageDTO
            {
                ReceiverId = "receiver-id",
                MessageContent = "Test message content"
            };

            var message = new Message
            {
                SenderId = "test-user-id",
                ReceiverId = sendMessageDto.ReceiverId,
                MessageContent = sendMessageDto.MessageContent,
                TimeStamp = DateTime.Now,
                IsRead = false
            };

            MockUnitOfWork.Setup(u => u.MessageRepository.AddAsync(It.IsAny<Message>()))
                         .ReturnsAsync(message);
            MockUnitOfWork.Setup(u => u.SaveAsync())
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SendMessage(sendMessageDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            MockUnitOfWork.Verify(u => u.MessageRepository.AddAsync(It.Is<Message>(m => 
                m.SenderId == "test-user-id" &&
                m.ReceiverId == sendMessageDto.ReceiverId &&
                m.MessageContent == sendMessageDto.MessageContent &&
                m.IsRead == false
            )), Times.Once);
            MockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [TestMethod]
        public async Task SendMessage_NullMessage_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.SendMessage(null!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("You Can't send Empty Message", badRequestResult.Value);
        }

        [TestMethod]
        public async Task SendMessage_NullMessageContent_ReturnsBadRequest()
        {
            // Arrange
            var sendMessageDto = new SendMessageDTO
            {
                ReceiverId = "receiver-id",
                MessageContent = null!
            };

            // Act
            var result = await _controller.SendMessage(sendMessageDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("You Can't send Empty Message", badRequestResult.Value);
        }

        [TestMethod]
        public async Task SendMessage_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var sendMessageDto = new SendMessageDTO
            {
                ReceiverId = "receiver-id",
                MessageContent = "Valid message"
            };

            _controller.ModelState.AddModelError("ReceiverId", "Required");

            // Act
            var result = await _controller.SendMessage(sendMessageDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Controllers don't implement IDisposable
        }
    }
}
