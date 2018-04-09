using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Contexts;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace Materialise.FrontendDays.Bot.Api.Repositories
{
    public class CategoryRepository : DbRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DbContextOptions<BotContext> contextOptions) : base(contextOptions)
        {
        }

        public async Task<KeyValuePair<User, Category>[]> GetUsersCategories()
        {
            using (var context = GetContext())
            {
                var usersGroup = context.UserAnswers
                    .Where(x => x.User.UserStatus == UserStatus.Answered)
                    .GroupBy(x => x.User);

                return await usersGroup
                    .Select(x => new KeyValuePair<User, Category>(x.Key, x.GroupBy(g => g.Answer.Category)
                        .MaxBy(g => g.Count())
                        .Key))
                    .ToArrayAsync();
            }
        }

        public Task<Category> GetUserCategory(int userId)
        {
            using (var context = GetContext())
            {
                var userAnswers = context.UserAnswers
                    .Where(x => x.UserId == userId && x.User.UserStatus == UserStatus.Answered);

                return Task.FromResult(userAnswers.GroupBy(x => x.Answer.Category)
                    .MaxBy(x => x.Count())
                    ?.Key);
            }
        }
    }
}
