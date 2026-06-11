using System.ComponentModel.DataAnnotations;

namespace AiChatBot.Api.Models;

public sealed class RenameRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
}
