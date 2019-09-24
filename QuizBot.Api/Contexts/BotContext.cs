using QuizBot.Api.Configurations;
using QuizBot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace QuizBot.Api.Contexts
{
    public sealed class BotContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<Category> Categories { get; set; }

        public BotContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserTypeConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserAnswerTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AnswerTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryTypeConfiguration());
        }
    }
}
