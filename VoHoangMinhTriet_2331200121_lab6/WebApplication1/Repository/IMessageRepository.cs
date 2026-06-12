using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Repository;

public interface IMessageRepository
{
    public Task<User?> GetUserByNameAsync(string name);
    public Task CreateMessageAsync(Message message);
    public Task<IEnumerable<Message>> GetBroadcastMessagesAsync();
    public Task<IEnumerable<Message>> GetGroupChatMessagesAsync(string name);
    public Task<IEnumerable<Message>> GetPrivateMessagesAsync(int senderId, int receiverId);
}