using Materialise.FrontendDays.Bot.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Materialise.FrontendDays.Bot.Api.Configurations
{
    public class StubAnswerTypeConfiguration : IEntityTypeConfiguration<StubAnswer>
    {
        public void Configure(EntityTypeBuilder<StubAnswer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Question)
                .WithOne()
                .HasForeignKey<StubAnswer>(x => x.QuestionId);
        }
    }
}
