using WebApplication1.Models;
using WebApplication1.Repository;

namespace WebApplication1.Service;

internal class MessageService : IMessageService
{
    private readonly IMessageRepository _repository;

    public MessageService(IMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task SaveMessageAsync(string senderName, string messageBody, int messageType,
        string? roomName, string? receiverName)
    {
        var sender = await _repository.GetUserByNameAsync(senderName);
        if (sender == null) throw new Exception("user not found");

        int receiverId = 0;
        if (messageType == 2 && !string.IsNullOrEmpty(receiverName))
        {
            var receiver = await _repository.GetUserByNameAsync(receiverName);
            if (receiver == null)
            {
                throw new Exception("receiver name not found");
                return;
            }
            receiverId = receiver.Id;
        }
        
        var message = new Message
        {
            SenderId = sender.Id,
            ReceiverId = receiverId == 0 ? null : receiverId,
            MessageType = (short)messageType,
            MessageBody = messageBody,
            RoomName = roomName,
            CreatedAt = DateTime.Now
        };
        await _repository.CreateMessageAsync(message);
        
    }                   
    
}