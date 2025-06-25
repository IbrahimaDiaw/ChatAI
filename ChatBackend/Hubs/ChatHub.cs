using ChatBackend.Services.AI;
using ChatBackend.Services.Chat;
using ChatShared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace ChatBackend.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IAIService _aiService;
        private readonly IChatSessionService _sessionService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IAIService aiService, IChatSessionService sessionService, ILogger<ChatHub> logger)
        {
            _aiService = aiService;
            _sessionService = sessionService;
            _logger = logger;
        }

        public async Task SendMessage(string user, string message, string sessionId)
        {
            try
            {
                _logger.LogInformation($"Processing message from {user} in session {sessionId}: {message}");

                // Validate input
                if (string.IsNullOrWhiteSpace(message))
                {
                    await Clients.Caller.SendAsync("ReceiveError", "Message cannot be empty.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                }

                // Add user message to session history
                var userMessage = new ChatMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    SessionId = sessionId,
                    User = user,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    IsFromAI = false
                };

                _sessionService.AddMessage(userMessage);

                // Get chat history for context
                var chatHistory = _sessionService.GetChatHistory(sessionId);

                // Generate AI response
                var aiResponse = await _aiService.GenerateResponseAsync(message, chatHistory);

                // Add AI response to session history
                var aiMessage = new ChatMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    SessionId = sessionId,
                    User = "AI Assistant",
                    Message = aiResponse,
                    Timestamp = DateTime.UtcNow,
                    IsFromAI = true
                };

                _sessionService.AddMessage(aiMessage);

                // Send response back to the client
                await Clients.All.SendAsync("ReceiveMessage", "AI Assistant", aiResponse, sessionId);

                _logger.LogInformation($"AI response sent for session {sessionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing message from {user} in session {sessionId}");
                await Clients.Caller.SendAsync("ReceiveError", "I encountered an error processing your request. Please try again.");
            }
        }

        public async Task JoinSession(string sessionId, string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

                _logger.LogInformation($"User {userName} joined session {sessionId}");

                // Send session joined confirmation
                await Clients.Caller.SendAsync("SessionJoined", sessionId);

                // Optionally notify other users in the session
                await Clients.OthersInGroup(sessionId).SendAsync("UserJoined", userName, sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error joining session {sessionId} for user {userName}");
                await Clients.Caller.SendAsync("ReceiveError", "Failed to join chat session.");
            }
        }

        public async Task LeaveSession(string sessionId, string userName)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);

                _logger.LogInformation($"User {userName} left session {sessionId}");

                // Notify other users in the session
                await Clients.OthersInGroup(sessionId).SendAsync("UserLeft", userName, sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error leaving session {sessionId} for user {userName}");
            }
        }

        public async Task GetChatHistory(string sessionId)
        {
            try
            {
                var history = _sessionService.GetChatHistory(sessionId);
                await Clients.Caller.SendAsync("ChatHistory", history, sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving chat history for session {sessionId}");
                await Clients.Caller.SendAsync("ReceiveError", "Failed to retrieve chat history.");
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            if (exception != null)
            {
                _logger.LogError(exception, "Client disconnected with exception");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
