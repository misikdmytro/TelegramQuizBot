using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Contexts;
using Materialise.FrontendDays.Bot.Api.Models;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace Materialise.FrontendDays.Bot.Api.Repositories
{
    public class CategoryRepository : DbRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DbContextOptions<BotContext> contextOptions) : base(contextOptions)
        {
        }

        public Task<Category> GetUserCategory(int userId)
        {
            using (var context = GetContext())
            {
                var userAnswers = context.UserAnswers
                    .Where(x => x.UserId == userId);

                return Task.FromResult(userAnswers.GroupBy(x => x.Answer.Category)
                    .MaxBy(x => x.Count())
                    .Key);
            }
        }
    }
}
