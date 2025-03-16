using System;

namespace SimpleChatAppWithBlazor.Shared.Models
{
    public class ChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
    }
}