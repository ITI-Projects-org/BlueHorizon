using System.Security.Claims;
using API.DTOs.MessageDTO;
using API.Models;
using API.UnitOfWorks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        public MessageController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            Mapper = mapper;
            UnitOfWork = unitOfWork;
        }

        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }

        // ✅ Get Inbox Messages for Current User
        [HttpGet("inbox")]
        public async Task<IActionResult> GetInbox()

        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // أو BadRequest حسب الـ Logic بتاعك
            }

            // الـ Repository هيرجع InboxItemDto مباشرة، مش محتاج Mapper هنا
            var inboxItems = await UnitOfWork.MessageRepository.GetInboxAsync(userId);

            return Ok(inboxItems);
        }

        // ✅ Get Chat Between Two Users
        [HttpGet("history")]
        public async Task<IActionResult> GetChat([FromQuery] string otherUserId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }
            if (string.IsNullOrEmpty(otherUserId))
            {
                return BadRequest("Other user ID is required.");
            }

            // الـ Repository هيرجع MessageDto مباشرة، مش محتاج Mapper هنا
            var messages = await UnitOfWork.MessageRepository.GetChatBetweenUsersAsync(currentUserId, otherUserId);

            return Ok(messages);
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage(SendMessageDTO messageDTO)
        {
            if (messageDTO == null || messageDTO.MessageContent == null)
            {
                return BadRequest("You Can't send Empty Message");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var message = new Message()
            {
                SenderId = userId,
                ReceiverId = messageDTO.ReceiverId,
                TimeStamp = DateTime.Now,
                MessageContent = messageDTO.MessageContent,
                IsRead = false,

            };
            await UnitOfWork.MessageRepository.AddAsync(message);
            await UnitOfWork.SaveAsync();
            return Ok();
        }
    }
}
