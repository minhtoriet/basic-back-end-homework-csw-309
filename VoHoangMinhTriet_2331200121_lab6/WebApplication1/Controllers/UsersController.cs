using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet("online")]
        public IActionResult GetOnlineUsers()
        {
            // Project the dictionary into a clean list of anonymous objects containing username + connectionId
            var userList = OnlineUserTracker.OnlineUsers.Select(u => new
            {
                Username = u.Key,
                ConnectionId = u.Value
            }).ToList();

            return Ok(userList);
        }
    }
}
