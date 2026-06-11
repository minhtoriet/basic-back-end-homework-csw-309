using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebApplication1.Models.Context;

namespace WebApplication1
{
    public class ChatHub : Hub
    {
        private readonly ChatDBContext _context;
        public ChatHub(ChatDBContext context) { _context = context; }

        //broadcast
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);

            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Name == user);
            _context.Messages.Add(new Models.Message 
                { SenderId = sender?.Id ?? 0, MessageBody = message, MessageType = 0 }); //0 for broadcast
            await _context.SaveChangesAsync();
        }
        // group chat
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }
        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
        public async Task SendGroupMessage(string roomName, string user, string message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveGroupMessage", user, message, roomName);
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Name == user);
            _context.Messages.Add(new Models.Message
                { SenderId = sender?.Id ?? 0, MessageBody = message, MessageType = 1 }); //1 for gc
            await _context.SaveChangesAsync();
        }
        //private chat
        public async Task SendPrivateMessage(string receiverConnectionId, string user, string message)
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", user, message);
            var sender = _context.Users.FirstOrDefault(u => u.Name == user);
            _context.Messages.Add(new Models.Message 
                {SenderId = sender?.Id ?? 0, MessageBody = message, MessageType = 2 });
            await _context.SaveChangesAsync();
        }
    }
}
