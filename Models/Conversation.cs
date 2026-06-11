using System.ComponentModel.DataAnnotations;

namespace AiChatBot.Api.Models;

public sealed class Conversation
{
    [Key]
    public Guid ConversationId { get; set; } = Guid.NewGuid();

    [MaxLength(200)]
    public string Title { get; set; } = "New Chat";

    public List<ChatMessage> Messages { get; set; } = [];

    // Audit Fields
    [MaxLength(100)]
    public string CreatedBy { get; set; } = "System";

    [MaxLength(100)]
    public string UpdatedBy { get; set; } = "System";

    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedDateUtc { get; set; } = DateTime.UtcNow;
}