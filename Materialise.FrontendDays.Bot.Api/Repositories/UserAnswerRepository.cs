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

        public async Task<Question[]> GetAnsweredQuestions(int userId)
        {
            using (var context = GetContext())
            {
                return await context.UserAnswers.Where(x => x.UserId == userId)
                    .Select(x => x.Answer.Question)
                    .ToArrayAsync();
            }
        }
    }
}
