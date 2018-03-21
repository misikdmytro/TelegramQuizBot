using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Contexts;
using Materialise.FrontendDays.Bot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Materialise.FrontendDays.Bot.Api.Repositories
{
    public class UserAnswerRepository : DbRepository<UserAnswer>, IUserAnswerRepository
    {
        public UserAnswerRepository(DbContextOptions<BotContext> contextOptions) : base(contextOptions)
        {
        }

        public async Task<UserAnswer[]> GetCorrectAnswers()
        {
            using (var context = GetContext())
            {
                return await GetQuery(context).Where(x => x.IsCorrect == true)
                    .ToArrayAsync();
            }
        }

        protected override IQueryable<UserAnswer> GetQuery(BotContext context)
        {
            return base.GetQuery(context)
                .Include(x => x.Question)
                .Include(x => x.User);
        }
    }
}
