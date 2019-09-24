using System.Linq;
using QuizBot.Api.Contexts;
using QuizBot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace QuizBot.Api.Repositories
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
