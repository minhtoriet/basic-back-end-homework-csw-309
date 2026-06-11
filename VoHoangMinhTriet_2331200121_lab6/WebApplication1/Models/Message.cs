using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Messages")]
    public class Message
    {
        public User Sender { get; set; }
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string RoomName { get; set; }
        public string MessageBody { get; set; }
        public short MessageType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
