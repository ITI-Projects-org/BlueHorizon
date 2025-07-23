using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class ChatHub : Hub
    {
        // دالة إرسال رسالة لمستخدم معيّن
        public async Task SendMessageToUser(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier; // دا هيرجع الـ nameid من التوكن
            if (senderId != null)
            {
                await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
            }
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"User Connected: {Context.UserIdentifier}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"User Disconnected: {Context.UserIdentifier}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
