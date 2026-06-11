namespace AiChatBot.Api.Models;

public sealed class ChatResponse
{
    public bool Success { get; init; }

    public Guid ConversationId { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? Error { get; init; }

    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
}