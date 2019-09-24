using QuizBot.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QuizBot.Api.Configurations
{
    public class AnswerTypeConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Text).IsRequired();
            builder.HasOne(x => x.Question)
                .WithMany(x => x.PossibleAnswers)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(x => x.QuestionId);
            builder.HasOne(x => x.Category)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(x => x.CategoryId);
            builder.Property(x => x.IsStub).IsRequired();
        }
    }
}
