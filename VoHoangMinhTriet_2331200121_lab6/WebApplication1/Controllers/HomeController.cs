using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View("~/Views/Home/Home.cshtml");
        }
        [HttpGet("/chat")]
        public IActionResult Chat()
        {
            return View("~/Views/Chat/Index.cshtml");
        }
        [HttpGet("/groupchat")]
        public IActionResult GroupChat()
        {
            return View("~/Views/Chat/GroupChat.cshtml");
        }
        [HttpGet("/privatechat")]
        public IActionResult PrivateChat()
        {
            return View("~/Views/Chat/PrivateChat.cshtml");
        }
        [HttpGet("/api/rooms")]
        public IActionResult GetRooms()
        {
            var rooms = new string[] { "General", "Gaming", "Programming", "Memes" };
            return Ok(rooms);
        }
    }
}
