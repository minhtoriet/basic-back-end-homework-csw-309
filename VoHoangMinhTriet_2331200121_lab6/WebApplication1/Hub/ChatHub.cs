using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Models.Context;
using WebApplication1.Service;
using WebApplication1.Services;

namespace WebApplication1.Hub
{
    [Authorize]
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly ChatDBContext _context;
        private readonly IMessageService _service;
        public ChatHub(ChatDBContext context, IMessageService service) 
        { 
            _context = context;
            _service = service;
        }

        public override async Task OnConnectedAsync()
        {
            string username = Context.User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            // Map/Update the user's active connection string
            OnlineUserTracker.OnlineUsers[username] = Context.ConnectionId;

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Context.User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            // Remove them from the active list
            OnlineUserTracker.OnlineUsers.TryRemove(username, out _);

            await base.OnDisconnectedAsync(exception);
        }
        //broadcast
        public async Task SendMessage(string message)
        {
            string senderName = Context.User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown Sender";
            int senderId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await Clients.All.SendAsync("ReceiveMessage", senderName, message);
            await _service.SaveMessageAsync(senderName, senderId, message, 0, null, null);
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
        public async Task SendGroupMessage(string roomName, string message)
        {
            string senderName = Context.User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown Sender";
            int senderId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await Clients.Group(roomName).SendAsync("ReceiveGroupMessage", senderName, message, roomName);
            await _service.SaveMessageAsync(senderName, senderId, message, 1, roomName, null);
        }
        //private chat
        public async Task SendPrivateMessage(string receiverConnectionId, string message, string receiverName)
        {
            string senderName = Context.User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown Sender";
            int senderId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", senderName, message);
            await Clients.Caller.SendAsync("ReceivePrivateMessage", senderName, message);
            await _service.SaveMessageAsync(senderName, senderId, message, 2, null, receiverName);
        }
    }
}
