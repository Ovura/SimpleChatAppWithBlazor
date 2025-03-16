using System;

namespace SimpleChatAppWithBlazor.Shared.Models
{
    public class ChatUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    }
}