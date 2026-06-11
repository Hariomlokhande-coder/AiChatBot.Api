using AiChatBot.Api.Models;

namespace AiChatBot.Api.Services;

public interface IOpenAiService
{
    Task<string> GetResponseAsync(
        List<ChatMessage> messages,
        CancellationToken cancellationToken = default);
}