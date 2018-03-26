using Materialise.FrontendDays.Bot.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Materialise.FrontendDays.Bot.Api.Configurations
{
    public class AnswerTypeConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Text).IsRequired();
            builder.Property(x => x.IsCorrect).IsRequired();
            builder.HasOne(x => x.Question)
                .WithMany(x => x.PossibleAnswers)
                .HasForeignKey(x => x.QuestionId);
        }
    }
}
