using System.ComponentModel.DataAnnotations;

namespace AiChatBot.Api.Models;

public sealed class ChatRequest
{
    [Required]
    [MaxLength(4000)]
    public string Message { get; set; } = string.Empty;

    public Guid? ConversationId { get; set; }
}