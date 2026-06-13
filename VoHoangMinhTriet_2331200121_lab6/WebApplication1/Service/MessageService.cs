using WebApplication1.Models;
using WebApplication1.Repository;

namespace WebApplication1.Service;

internal class MessageService : IMessageService
{
    private readonly IMessageRepository _repository;
    private readonly IUserRepository _userRepo;

    public MessageService(IMessageRepository repository, IUserRepository userRepo)
    {
        _repository = repository;
        _userRepo = userRepo;
    }

    public async Task SaveMessageAsync(string senderName,int senderId, string messageBody, int messageType,
        string? roomName, string? receiverName)
    {
        
        int receiverId = 0;

        // receiverName was sent as fooking email. Speechless.
        if (messageType == 2 && !string.IsNullOrEmpty(receiverName))
        {
            var receiver = await _userRepo.GetUserByEmailAsync(receiverName);
            if (receiver == null)
            {
                throw new Exception("receiver name not found");
            }
            receiverId = receiver.Id;
        }
        
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId == 0 ? null : receiverId,
            MessageType = (short)messageType,
            MessageBody = messageBody,
            RoomName = roomName,
            CreatedAt = DateTime.Now
        };
        await _repository.CreateMessageAsync(message);
        
    }                   
    
}