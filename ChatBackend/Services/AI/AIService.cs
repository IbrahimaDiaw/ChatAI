using ChatShared.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ChatBackend.Services.AI
{
    public class AIService : IAIService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<AIService> _logger;
        private const string DefaultSystemPrompt = @"You are a helpful AI assistant. You provide accurate, helpful, and friendly responses to user questions. 
Keep your responses concise but informative. If you're unsure about something, say so rather than guessing.";

        public AIService(Kernel kernel, ILogger<AIService> logger)
        {
            _kernel = kernel;
            _logger = logger;
        }

        public async Task<string> GenerateResponseAsync(string userMessage, List<ChatMessage> chatHistory)
        {
            return await GenerateResponseWithSystemPromptAsync(userMessage, DefaultSystemPrompt, chatHistory);
        }

        public async Task<string> GenerateResponseWithSystemPromptAsync(string userMessage, string systemPrompt, List<ChatMessage> chatHistory)
        {
            try
            {
                var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
                var history = new ChatHistory(systemPrompt);

                // Add recent chat history for context (limit to last 10 messages to avoid token limits)
                var recentHistory = chatHistory
                    .OrderBy(m => m.Timestamp)
                    .TakeLast(10)
                    .ToList();

                foreach (var message in recentHistory)
                {
                    if (message.IsFromAI)
                    {
                        history.AddAssistantMessage(message.Message);
                    }
                    else
                    {
                        history.AddUserMessage(message.Message);
                    }
                }

                // Add the current user message
                history.AddUserMessage(userMessage);

                _logger.LogInformation($"Generating AI response for message: {userMessage}");

                // Generate response
                var response = await chatCompletionService.GetChatMessageContentAsync(history);

                _logger.LogInformation("AI response generated successfully");

                return response.Content ?? "I apologize, but I couldn't generate a response. Please try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI response");
                return "I'm sorry, I encountered an error while processing your request. Please try again later.";
            }
        }
    }
}
