using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuizBot.Api.Contexts;
using QuizBot.Api.Models;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using QuizBot.Api.Repositories.Interfaces;

namespace QuizBot.Api.Repositories
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
                var usersGroup = await GetAnswered(context)
                    .GroupBy(x => x.User)
                    .ToArrayAsync();

                return usersGroup
                    .Select(x => new KeyValuePair<User, Category>(x.Key, x.GroupBy(g => g.Answer.Category)
                        .MaxBy(g => g.Count())
                        .Key))
                    .ToArray();
            }
        }

        public Task<Category> GetUserCategory(int userId)
        {
            using (var context = GetContext())
            {
                var userAnswers = GetAnswered(context)
                    .Where(x => x.UserId == userId);

                return Task.FromResult(userAnswers.GroupBy(x => x.Answer.Category)
                    .MaxBy(x => x.Count())
                    .Key);
            }
        }

        private IQueryable<UserAnswer> GetAnswered(BotContext context)
        {
            return context.UserAnswers
                .Include(x => x.Answer)
                .Include(x => x.Answer.Category)
                .Where(x => x.User.UserStatus == UserStatus.Answered);
        }
    }
}
