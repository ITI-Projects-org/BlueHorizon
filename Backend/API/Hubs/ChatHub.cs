using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Linq;
using API.UnitOfWorks;
using API.Models;
using System;
using Microsoft.AspNetCore.Authorization;

namespace API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SendMessage(string receiverId, string messageContent)
        {
            var senderId = Context.UserIdentifier;
            Console.WriteLine($"[ChatHub] SendMessageToUser received: SenderId = '{senderId}', ReceiverId = '{receiverId}', MessageContent = '{messageContent}'");

            if (senderId != null)
            {
                var message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    MessageContent = messageContent,
                    TimeStamp = DateTime.Now,
                    IsRead = false
                };

                await _unitOfWork.MessageRepository.AddAsync(message);
                await _unitOfWork.SaveAsync();
                Console.WriteLine($"[ChatHub] Message saved to database: From '{senderId}' to '{receiverId}'");


                var clientProxy = Clients.User(receiverId);
                if (clientProxy == null)
                {
                    Console.WriteLine($"[ChatHub] WARNING: Receiver '{receiverId}' not found or not currently connected to this Hub instance. Message saved but not sent in real-time.");
                }
                else
                {

                    await clientProxy.SendAsync("ReceiveMessage", new
                    {
                        Id = message.Id,
                        SenderId = message.SenderId,
                        ReceiverId = message.ReceiverId,
                        MessageContent = message.MessageContent,
                        TimeStamp = message.TimeStamp,
                        IsRead = message.IsRead
                    });
                    Console.WriteLine($"[ChatHub] Message successfully sent in real-time to '{receiverId}' from '{senderId}'.");
                }
            }
            else
            {
                Console.WriteLine("[ChatHub] ERROR: SenderId is null. User might not be authenticated correctly for the Hub.");
            }
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"[ChatHub] User Connected: '{Context.UserIdentifier}' (Connection ID: {Context.ConnectionId})");

            if (Context.User != null && Context.User.Claims != null && Context.User.Claims.Any())
            {
                Console.WriteLine("[ChatHub] User Claims on connection:");
                foreach (var claim in Context.User.Claims)
                {
                    Console.WriteLine($"  - Type: {claim.Type}, Value: {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("[ChatHub] No claims found for connected user (Context.User is null or has no claims). This indicates an authentication issue.");
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"[ChatHub] User Disconnected: '{Context.UserIdentifier}' (Connection ID: {Context.ConnectionId})");
            if (exception != null)
            {
                Console.WriteLine($"[ChatHub] Disconnection Reason: {exception.Message}");
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}