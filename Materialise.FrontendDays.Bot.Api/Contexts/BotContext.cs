using Materialise.FrontendDays.Bot.Api.Configurations;
using Materialise.FrontendDays.Bot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Materialise.FrontendDays.Bot.Api.Contexts
{
    public class BotContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        public BotContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserTypeConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserAnswerTypeConfiguration());
        }
    }
}
