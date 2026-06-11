using System.ComponentModel.DataAnnotations;

namespace AiChatBot.Api.Models;

public sealed class ChatMessage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    //public string ConversationId { get; set; } = string.Empty;
    public Guid ConversationId { get; set; }

    public Conversation? Conversation { get; set; }

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string Content { get; set; } = string.Empty;

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    // Audit Fields
    [MaxLength(100)]
    public string CreatedBy { get; set; } = "System";

    [MaxLength(100)]
    public string UpdatedBy { get; set; } = "System";

    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedDateUtc { get; set; } = DateTime.UtcNow;
}