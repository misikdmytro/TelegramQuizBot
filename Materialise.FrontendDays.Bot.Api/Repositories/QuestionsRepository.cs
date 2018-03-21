using System.Linq;
using Materialise.FrontendDays.Bot.Api.Contexts;
using Materialise.FrontendDays.Bot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Materialise.FrontendDays.Bot.Api.Repositories
{
    public class QuestionsRepository : DbRepository<Question>
    {
        public QuestionsRepository(DbContextOptions<BotContext> contextOptions) : base(contextOptions)
        {
        }

        protected override IQueryable<Question> GetQuery(BotContext context)
        {
            return base.GetQuery(context).Include(x => x.PossibleAnswers);
        }
    }
}
