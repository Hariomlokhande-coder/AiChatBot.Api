using System;

namespace AiChatBot.Api.Models;

public sealed class ConversationSummary
{
    public Guid ConversationId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime CreatedDateUtc { get; set; }
}
