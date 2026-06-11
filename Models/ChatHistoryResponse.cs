using System;
using System.Collections.Generic;

namespace AiChatBot.Api.Models;

public sealed class ChatHistoryResponse
{
    public List<ChatMessageResponse> Messages { get; set; } = [];
    public bool HasMore { get; set; }
}

public sealed class ChatMessageResponse
{
    public Guid Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
