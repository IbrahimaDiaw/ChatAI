using ChatShared.Models;

namespace ChatBackend.Services.Chat
{
    public interface IChatSessionService
    {
        void AddMessage(ChatMessage message);
        List<ChatMessage> GetChatHistory(string sessionId);
        ChatSession? GetSession(string sessionId);
        void CreateSession(string sessionId, string? title = null);
        void ClearSession(string sessionId);
        List<ChatSession> GetAllSessions();
        void CleanupOldSessions();
    }
}
