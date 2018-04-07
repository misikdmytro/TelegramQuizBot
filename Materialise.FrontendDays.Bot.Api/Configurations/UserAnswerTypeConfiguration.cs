using Materialise.FrontendDays.Bot.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Materialise.FrontendDays.Bot.Api.Configurations
{
    public class UserAnswerTypeConfiguration : IEntityTypeConfiguration<UserAnswer>
    {
        public void Configure(EntityTypeBuilder<UserAnswer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Question)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(x => x.QuestionId);
            builder.HasOne(x => x.Answer)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(x => x.AnswerId);
        }
    }
}
