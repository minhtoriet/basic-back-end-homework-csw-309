namespace WebApplication1.Service;

public interface IMessageService
{
    public Task SaveMessageAsync(string senderName, int senderId, string messageBody, int messageType,
        string? roomName, string? receiverName);
    
}