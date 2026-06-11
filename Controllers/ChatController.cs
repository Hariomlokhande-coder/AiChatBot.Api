using AiChatBot.Api.Data;
using AiChatBot.Api.Models;
using AiChatBot.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AiChatBot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ChatController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IOpenAiService _openAiService;

    public ChatController(
        ApplicationDbContext dbContext,
        IOpenAiService openAiService)
    {
        _dbContext = dbContext;
        _openAiService = openAiService;
    }

    [HttpPost("send")]
    public async Task<ActionResult<ChatResponse>> SendMessage(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guid conversationId =
                request.ConversationId ?? Guid.NewGuid();

            // Create conversation if it doesn't exist
            var conversation = await _dbContext.Conversations
                .FirstOrDefaultAsync(
                    x => x.ConversationId == conversationId,
                    cancellationToken);

            if (conversation is null)
            {
                conversation = new Conversation
                {
                    ConversationId = conversationId,
                    Title = request.Message.Length > 50
                        ? request.Message[..50]
                        : request.Message
                };

                _dbContext.Conversations.Add(conversation);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            var userMessage = new ChatMessage
            {
                ConversationId = conversationId,
                Role = "user",
                Content = request.Message
            };

            _dbContext.ChatMessages.Add(userMessage);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var conversationHistory =
                await _dbContext.ChatMessages
                    .Where(x => x.ConversationId == conversationId)
                    .OrderBy(x => x.TimestampUtc)
                    .ToListAsync(cancellationToken);

            var aiResponse =
                await _openAiService.GetResponseAsync(
                    conversationHistory,
                    cancellationToken);

            var assistantMessage = new ChatMessage
            {
                ConversationId = conversationId,
                Role = "assistant",
                Content = aiResponse
            };

            _dbContext.ChatMessages.Add(assistantMessage);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new ChatResponse
            {
                Success = true,
                ConversationId = conversationId,
                Message = aiResponse
            });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    Success = false,
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
        }
    }

    [HttpGet("{conversationId:guid}")]
    public async Task<IActionResult> GetHistory(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        var messages = await _dbContext.ChatMessages
            .Where(x => x.ConversationId == conversationId)
            .OrderBy(x => x.TimestampUtc)
            .ToListAsync(cancellationToken);

        // Map stored roles to frontend-friendly roles ('user' | 'ai')
        var mapped = messages.Select(m => new
        {
            id = m.Id,
            role = m.Role == "assistant" ? "ai" : m.Role,
            text = m.Content,
            timestamp = m.TimestampUtc
        });

        return Ok(mapped);
    }

    [HttpGet("{conversationId:guid}/history")]
    public async Task<ActionResult<ChatHistoryResponse>> GetHistoryPaginated(
        Guid conversationId,
        [FromQuery] DateTime? before,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ChatMessages
            .Where(x => x.ConversationId == conversationId);

        if (before.HasValue)
        {
            query = query.Where(x => x.TimestampUtc < before.Value);
        }

        // Fetch pageSize + 1 items to see if there is another page
        var messages = await query
            .OrderByDescending(x => x.TimestampUtc)
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);
        bool hasMore = messages.Count > pageSize;
        if (hasMore)
        {
            messages.RemoveAt(messages.Count - 1);
        }

        // Map and reverse list back to chronological order (ascending) for presentation
        var mapped = messages
            .OrderBy(m => m.TimestampUtc)
            .Select(m => new ChatMessageResponse
            {
                Id = m.Id,
                Role = m.Role == "assistant" ? "ai" : m.Role,
                Text = m.Content,
                Timestamp = m.TimestampUtc
            })
            .ToList();

        return Ok(new ChatHistoryResponse
        {
            Messages = mapped,
            HasMore = hasMore
        });
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations(CancellationToken cancellationToken)
    {
        var list = await _dbContext.Conversations
            .OrderByDescending(x => x.CreatedDateUtc)
            .Select(c => new ConversationSummary
            {
                ConversationId = c.ConversationId,
                Title = c.Title,
                CreatedDateUtc = c.CreatedDateUtc
            })
            .ToListAsync(cancellationToken);

        return Ok(list);
    }

    [HttpDelete("{conversationId:guid}")]
    public async Task<IActionResult> NewChat(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        var messages = await _dbContext.ChatMessages
            .Where(x => x.ConversationId == conversationId)
            .ToListAsync(cancellationToken);

        if (messages.Count > 0)
        {
            _dbContext.ChatMessages.RemoveRange(messages);
        }

        var conversation = await _dbContext.Conversations
            .FirstOrDefaultAsync(
                x => x.ConversationId == conversationId,
                cancellationToken);

        if (conversation is not null)
        {
            _dbContext.Conversations.Remove(conversation);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPatch("{conversationId:guid}/rename")]
    public async Task<IActionResult> RenameConversation(
        Guid conversationId,
        [FromBody] RenameRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var conversation = await _dbContext.Conversations
            .FirstOrDefaultAsync(c => c.ConversationId == conversationId, cancellationToken);

        if (conversation is null)
            return NotFound();
        conversation.Title = request.Title;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}