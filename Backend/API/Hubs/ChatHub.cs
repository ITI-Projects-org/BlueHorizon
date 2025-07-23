using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Linq;
using API.UnitOfWorks; // ✅ تم إضافة هذا الـ using
using API.Models;     // ✅ تم إضافة هذا الـ using لـ Message Model
using System;
using Microsoft.AspNetCore.Authorization;         // ✅ تم إضافة هذا الـ using لـ DateTime.Now

namespace API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        // ✅ حقن (Dependency Injection) لـ IUnitOfWork
        private readonly IUnitOfWork _unitOfWork;

        // ✅ Constructor لاستقبال الـ IUnitOfWork المحقون
        public ChatHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // دالة إرسال رسالة لمستخدم معيّن
        public async Task SendMessageToUser(string receiverId, string messageContent) // ✅ تم تغيير اسم المتغير لـ messageContent عشان ميتعارضش مع الـ Message Model
        {
            var senderId = Context.UserIdentifier;
            Console.WriteLine($"[ChatHub] SendMessageToUser received: SenderId = '{senderId}', ReceiverId = '{receiverId}', MessageContent = '{messageContent}'");

            if (senderId != null)
            {
                // 1. ✅ حفظ الرسالة في قاعدة البيانات
                var message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    MessageContent = messageContent,
                    TimeStamp = DateTime.Now, // ✅ استخدام DateTime.UtcNow لأفضل ممارسة (التاريخ العالمي)
                    IsRead = false // الرسالة لسه متبعتتش، فـ IsRead تكون False في البداية
                };

                await _unitOfWork.MessageRepository.AddAsync(message); // إضافة الرسالة للـ Repository
                await _unitOfWork.SaveAsync(); // حفظ التغييرات في قاعدة البيانات
                Console.WriteLine($"[ChatHub] Message saved to database: From '{senderId}' to '{receiverId}'");

                // 2. ✅ إرسال الرسالة فوراً لمستخدم واحد (المستقبل)
                // ممكن تبعت الـ message object كله لو الـ Frontend بتاعك بيتعامل معاه
                // أو تبعت الـ senderId والـ messageContent زي ما كنت عامل
                var clientProxy = Clients.User(receiverId);
                if (clientProxy == null)
                {
                    Console.WriteLine($"[ChatHub] WARNING: Receiver '{receiverId}' not found or not currently connected to this Hub instance. Message saved but not sent in real-time.");
                    // ممكن هنا نضيف لوج آخر أو آلية إعادة محاولة إرسال
                }
                else
                {
                    // ✅ إرسال الرسالة للـ Frontend.
                    // الأفضل إنك ترجع الـ message object اللي تم حفظه، عشان الـ Frontend ياخد الـ TimeStamp والـ ID
                    // أو ممكن تبعت اللي كنت بتبعته: SendAsync("ReceiveMessage", senderId, messageContent)
                    // لكن لو الـ Frontend بيستقبل DTO، يبقى نرجع DTO:
                    // بما إن الـ Hub ملوش Mapper، هنبعت الـ properties اللي محتاجينها.
                    await clientProxy.SendAsync("ReceiveMessage", new
                    {
                        Id = message.Id, // ID الرسالة من الداتا بيز
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