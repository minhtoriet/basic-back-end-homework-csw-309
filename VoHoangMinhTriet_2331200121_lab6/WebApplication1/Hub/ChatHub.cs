using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebApplication1.Models.Context;
using WebApplication1.Service;

namespace WebApplication1.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly ChatDBContext _context;
        private readonly IMessageService _service;
        public ChatHub(ChatDBContext context, IMessageService service) 
        { 
            _context = context;
            _service = service;
        }

        //broadcast
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            await _service.SaveMessageAsync(user, message, 0, null, null);
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
            await _service.SaveMessageAsync(user, message, 1, roomName, null);
        }
        //private chat
        public async Task SendPrivateMessage(string receiverConnectionId, string user, string message, string receiverName)
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", user, message);
            await _service.SaveMessageAsync(user, message, 2, null, receiverName);
        }
    }
}
