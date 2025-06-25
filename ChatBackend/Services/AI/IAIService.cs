using ChatShared.Models;

namespace ChatBackend.Services.AI
{
    public interface IAIService
    {
        Task<string> GenerateResponseAsync(string userMessage, List<ChatMessage> chatHistory);
        Task<string> GenerateResponseWithSystemPromptAsync(string userMessage, string systemPrompt, List<ChatMessage> chatHistory);
    }
}
