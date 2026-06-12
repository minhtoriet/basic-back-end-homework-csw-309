using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.Context;

namespace WebApplication1.Repository;

internal class MessageRepository : IMessageRepository
{
    private readonly ChatDBContext _context;

    public MessageRepository(ChatDBContext context)
    {
        _context = context;
    }

    public async Task CreateMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByNameAsync(string name)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task<IEnumerable<Message>> GetBroadcastMessagesAsync()
    {
        return await _context.Messages.AsNoTracking()
            .Where(m => m.MessageType == 0)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetGroupChatMessagesAsync(string roomName)
    {
        return await _context.Messages.AsNoTracking()
            .Where(m => m.MessageType == 1 && m.RoomName == roomName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetPrivateMessagesAsync(int senderId, int receiverId)
    {
        return await _context.Messages.AsNoTracking()
            .Where(m => m.MessageType == 2 && m.SenderId == senderId && m.ReceiverId == receiverId)
            .ToListAsync();
    }
}