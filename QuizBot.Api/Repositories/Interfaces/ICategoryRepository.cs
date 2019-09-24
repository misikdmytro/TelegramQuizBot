using System.Collections.Generic;
using System.Threading.Tasks;
using QuizBot.Api.Models;

namespace QuizBot.Api.Repositories.Interfaces
{
    public interface ICategoryRepository : IDbRepository<Category>
    {
        Task<KeyValuePair<User, Category>[]> GetUsersCategories();
        Task<Category> GetUserCategory(int userId);
    }
}
