using SimpleChatAppWithBlazor.Shared.Models;

namespace SimpleChatAppWithBlazor.Services
{
    public interface IChatService
    {
        event Action<ChatMessage>? OnMessageReceived;
        event Action<ChatUser>? OnUserConnected;
        event Action<string>? OnUserDisconnected;

        Task ConnectAsync(string userName);
        Task DisconnectAsync();
        Task SendMessageAsync(string message);
        Task<IEnumerable<ChatUser>> GetOnlineUsersAsync();
        Task<IEnumerable<ChatMessage>> GetMessageHistoryAsync();
    }
}