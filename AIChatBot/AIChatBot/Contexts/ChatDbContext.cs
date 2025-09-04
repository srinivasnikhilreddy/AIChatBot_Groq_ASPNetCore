using Microsoft.EntityFrameworkCore;
using AIChatBot.Models;

namespace AIChatBot.Contexts
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        public DbSet<ChatHistory> ChatHistories { get; set; }
    }
}
