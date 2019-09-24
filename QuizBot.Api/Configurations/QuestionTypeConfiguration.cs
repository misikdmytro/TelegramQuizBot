using QuizBot.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QuizBot.Api.Configurations
{
    public class QuestionTypeConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Text).IsRequired();
            builder.HasMany(x => x.PossibleAnswers)
                .WithOne(x => x.Question)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(x => x.QuestionId);
        }
    }
}
