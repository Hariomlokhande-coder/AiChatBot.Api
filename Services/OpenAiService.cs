using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AiChatBot.Api.Models;
using AiChatBot.Api.Settings;
using Microsoft.Extensions.Options;

namespace AiChatBot.Api.Services;

public sealed class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiSettings _settings;
    private readonly ILogger<OpenAiService> _logger;

    public OpenAiService(
        HttpClient httpClient,
        IOptions<OpenAiSettings> options,
        ILogger<OpenAiService> logger)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<string> GetResponseAsync(
        List<ChatMessage> messages,
        CancellationToken cancellationToken = default)
    {
        if (!messages.Any())
            throw new ArgumentException("Conversation history is empty.");

        var requestBody = new
        {
            model = _settings.Model,
            messages = messages.Select(x => new
            {
                role = x.Role,
                content = x.Content
            })
        };

        var requestJson = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.groq.com/openai/v1/chat/completions");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _settings.ApiKey);

        request.Content = new StringContent(
            requestJson,
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        var responseContent =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Groq API Error: {Response}",
                responseContent);

            throw new ApplicationException(
                $"Groq API Error: {responseContent}");
        }

        using var document =
            JsonDocument.Parse(responseContent);

        var aiResponse =
            document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

        return string.IsNullOrWhiteSpace(aiResponse)
            ? "No response received."
            : aiResponse.Trim();
    }
}