using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models.Context
{
    public class ChatDBContext : DbContext
    {
        public ChatDBContext(DbContextOptions<ChatDBContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // for user
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<User>().Property(u => u.HashPassword).IsRequired().HasMaxLength(70);
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(120);
            modelBuilder.Entity<User>().Property(u => u.IsActive).IsRequired();

            // for message
            modelBuilder.Entity<Message>().HasKey(m => m.Id);
            modelBuilder.Entity<Message>().HasOne(m => m.Sender).WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>().Property(m => m.SenderId).IsRequired();
            modelBuilder.Entity<Message>().Property(m => m.RoomName).HasMaxLength(50);
            modelBuilder.Entity<Message>().Property(m => m.MessageBody).IsRequired();
            modelBuilder.Entity<Message>().Property(m => m.MessageType).IsRequired();
            modelBuilder.Entity<Message>().Property(m => m.CreatedAt).IsRequired();
        }
    }
}
