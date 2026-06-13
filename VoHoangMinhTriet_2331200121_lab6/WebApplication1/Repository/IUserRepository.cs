using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public interface IUserRepository
    {
        public Task<User?> GetUserByEmailAsync(string email);
        public Task CreateUserAsync(User user);
    }
}
