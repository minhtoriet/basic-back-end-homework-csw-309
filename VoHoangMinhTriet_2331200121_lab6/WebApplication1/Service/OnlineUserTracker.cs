using System.Collections.Concurrent;

namespace WebApplication1.Services
{
    public static class OnlineUserTracker
    {
        // Username -> ConnectionId
        public static ConcurrentDictionary<string, string> OnlineUsers = new ConcurrentDictionary<string, string>();
    }
}