using ChatShared.Models;
using System.Collections.Concurrent;

namespace ChatBackend.Services.Chat
{
    public class ChatSessionService : IChatSessionService
    {
        private readonly ConcurrentDictionary<string, ChatSession> _sessions = new();
        private readonly ILogger<ChatSessionService> _logger;
        private readonly Timer _cleanupTimer;

        public ChatSessionService(ILogger<ChatSessionService> logger)
        {
            _logger = logger;

            // Setup cleanup timer to run every hour
            _cleanupTimer = new Timer(
                callback: _ => CleanupOldSessions(),
                state: null,
                dueTime: TimeSpan.FromHours(1),
                period: TimeSpan.FromHours(1)
            );
        }

        public void AddMessage(ChatMessage message)
        {
            var session = _sessions.GetOrAdd(message.SessionId, _ => new ChatSession
            {
                Id = message.SessionId,
                CreatedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow
            });

            session.Messages.Add(message);
            session.LastActivity = DateTime.UtcNow;

            _logger.LogInformation($"Added message to session {message.SessionId}. Total messages: {session.Messages.Count}");
        }

        public List<ChatMessage> GetChatHistory(string sessionId)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                return session.Messages.OrderBy(m => m.Timestamp).ToList();
            }

            return new List<ChatMessage>();
        }

        public ChatSession? GetSession(string sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public void CreateSession(string sessionId, string? title = null)
        {
            var session = new ChatSession
            {
                Id = sessionId,
                Title = title,
                CreatedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow
            };

            _sessions.TryAdd(sessionId, session);
            _logger.LogInformation($"Created new session: {sessionId}");
        }

        public void ClearSession(string sessionId)
        {
            if (_sessions.TryRemove(sessionId, out var session))
            {
                _logger.LogInformation($"Cleared session {sessionId} with {session.Messages.Count} messages");
            }
        }

        public List<ChatSession> GetAllSessions()
        {
            return _sessions.Values.ToList();
        }

        public void CleanupOldSessions()
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-24); // Remove sessions inactive for 24 hours
            var sessionsToRemove = _sessions.Values
                .Where(s => s.LastActivity < cutoffTime)
                .Select(s => s.Id)
                .ToList();

            foreach (var sessionId in sessionsToRemove)
            {
                _sessions.TryRemove(sessionId, out _);
            }

            if (sessionsToRemove.Any())
            {
                _logger.LogInformation($"Cleaned up {sessionsToRemove.Count} old sessions");
            }
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }
    }
}
