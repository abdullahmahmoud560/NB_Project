using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NB_Project.ApplicationDbContext;
using NB_Project.Model;

namespace NB_Project.Services
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, string> Users = new();
        private readonly DB _db;

        public ChatHub(DB db)
        {
            _db = db;
        }

        public override Task OnConnectedAsync()
        {
            var token = Context.GetHttpContext()?.Request.Query["access_token"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                var userId = GetUserIdFromToken(token);
                if (!string.IsNullOrEmpty(userId))
                {
                    Users[userId] = Context.ConnectionId;
                }
            }

            return base.OnConnectedAsync();
        }

        private string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token.Replace("Bearer ", "")) as JwtSecurityToken;
            var userId = jsonToken?.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            return userId!;
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var Id = Users.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(Id))
            {
                Users.TryRemove(Id, out _);
                Console.WriteLine($"[Disconnected] User ID: {Id}");
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToUser(string toId, string message)
        {
            var senderId = Context.User?.FindFirst("id")?.Value;

            if (senderId == null)
                return;

            // 1. حفظ الرسالة
            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = toId,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            _db.chatMessages.Add(chatMessage);

            // 2. تحديد الترتيب الموحد للمستخدمين لمنع تكرار السجل
            var user1 = string.Compare(senderId, toId) < 0 ? senderId : toId;
            var user2 = string.Compare(senderId, toId) < 0 ? toId : senderId;

            var conversation = await _db.chatSummaries
                .FirstOrDefaultAsync(c => c.User1Id == user1 && c.User2Id == user2);

            if (conversation == null)
            {
                // إنشاء ملخص جديد
                conversation = new ChatSummary
                {
                    User1Id = user1,
                    User2Id = user2,
                    LastMessage = message,
                    LastMessageTime = DateTime.UtcNow,

                };

                _db.chatSummaries.Add(conversation);
            }
            else
            {
                // تحديث الملخص
                conversation.LastMessage = message;
                conversation.LastMessageTime = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            // 3. إرسال الرسالة للمستلم إن كان متصل
            if (Users.TryGetValue(toId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
        }

    }
}
