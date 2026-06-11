using AiChatBot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AiChatBot.Api.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ChatMessage> ChatMessages =>
        Set<ChatMessage>();

    public DbSet<Conversation> Conversations =>
        Set<Conversation>();

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is ChatMessage ||
                e.Entity is Conversation);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                switch (entry.Entity)
                {
                    case ChatMessage chatMessage:
                        chatMessage.CreatedDateUtc = DateTime.UtcNow;
                        chatMessage.UpdatedDateUtc = DateTime.UtcNow;
                        break;

                    case Conversation conversation:
                        conversation.CreatedDateUtc = DateTime.UtcNow;
                        conversation.UpdatedDateUtc = DateTime.UtcNow;
                        break;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                switch (entry.Entity)
                {
                    case ChatMessage chatMessage:
                        chatMessage.UpdatedDateUtc = DateTime.UtcNow;
                        break;

                    case Conversation conversation:
                        conversation.UpdatedDateUtc = DateTime.UtcNow;
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}