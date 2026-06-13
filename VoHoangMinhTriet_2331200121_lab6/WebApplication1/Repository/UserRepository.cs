using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.Context;

namespace WebApplication1.Repository
{
    internal class UserRepository : IUserRepository
    {
        private readonly ChatDBContext _context;

        public UserRepository(ChatDBContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        { 
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email.Trim());
        }
        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
