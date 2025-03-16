using Microsoft.AspNetCore.SignalR;
using SimpleChatAppWithBlazor.Shared.Models;

namespace SimpleChatAppWithBlazor.Server.Hubs;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, ChatUser> ConnectedUsers = new();
    private static readonly List<ChatMessage> MessageHistory = new();
    private const int MaxMessageHistory = 50;

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (ConnectedUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            ConnectedUsers.Remove(Context.ConnectionId);
            await Clients.Others.SendAsync("UserDisconnected", user.Id);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChat(string userName)
    {
        var user = new ChatUser
        {
            Id = Context.ConnectionId,
            UserName = userName,
            IsOnline = true,
            LastSeen = DateTime.UtcNow
        };

        ConnectedUsers[Context.ConnectionId] = user;
        await Clients.All.SendAsync("UserConnected", user);

        // Send existing messages to the new user
        foreach (var message in MessageHistory)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }
    }

    public async Task SendMessage(string content)
    {
        if (ConnectedUsers.TryGetValue(Context.ConnectionId, out var sender))
        {
            var message = new ChatMessage
            {
                SenderId = sender.Id,
                SenderName = sender.UserName,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            MessageHistory.Add(message);
            if (MessageHistory.Count > MaxMessageHistory)
            {
                MessageHistory.RemoveAt(0);
            }

            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }

    public async Task<IEnumerable<ChatUser>> GetOnlineUsers()
    {
        return await Task.FromResult(ConnectedUsers.Values);
    }

    public async Task<IEnumerable<ChatMessage>> GetMessageHistory()
    {
        return await Task.FromResult(MessageHistory);
    }
}