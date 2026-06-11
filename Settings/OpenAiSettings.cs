namespace AiChatBot.Api.Settings;

public sealed class OpenAiSettings
{
    public const string SectionName = "OpenAI";

    public string ApiKey { get; init; } = string.Empty;

    public string Model { get; init; } = "llama-3.3-70b-versatile";

    public int MaxHistoryMessages { get; init; } = 20;
}