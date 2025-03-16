using Microsoft.AspNetCore.SignalR.Client;
using SimpleChatAppWithBlazor.Shared.Models;

namespace SimpleChatAppWithBlazor.Services
{
    public class ChatService : IChatService, IAsyncDisposable
    {
        private readonly string _hubUrl;
        private HubConnection? _hubConnection;
        private readonly List<ChatMessage> _messageHistory;
        private readonly List<ChatUser> _onlineUsers;

        public event Action<ChatMessage>? OnMessageReceived;
        public event Action<ChatUser>? OnUserConnected;
        public event Action<string>? OnUserDisconnected;

        public ChatService(string hubUrl)
        {
            _hubUrl = hubUrl;
            _messageHistory = new List<ChatMessage>();
            _onlineUsers = new List<ChatUser>();
        }

        public async Task ConnectAsync(string userName)
        {
            if (_hubConnection != null)
                return;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<ChatMessage>("ReceiveMessage", (message) =>
            {
                _messageHistory.Add(message);
                OnMessageReceived?.Invoke(message);
            });

            _hubConnection.On<ChatUser>("UserConnected", (user) =>
            {
                if (!_onlineUsers.Any(u => u.Id == user.Id))
                {
                    _onlineUsers.Add(user);
                    OnUserConnected?.Invoke(user);
                }
            });

            _hubConnection.On<string>("UserDisconnected", (userId) =>
            {
                var user = _onlineUsers.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    _onlineUsers.Remove(user);
                    OnUserDisconnected?.Invoke(userId);
                }
            });

            await _hubConnection.StartAsync();
            await _hubConnection.SendAsync("JoinChat", userName);
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.SendAsync("SendMessage", message);
            }
        }

        public Task<IEnumerable<ChatUser>> GetOnlineUsersAsync()
        {
            return Task.FromResult(_onlineUsers.AsEnumerable());
        }

        public Task<IEnumerable<ChatMessage>> GetMessageHistoryAsync()
        {
            return Task.FromResult(_messageHistory.AsEnumerable());
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}