using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;

namespace Materialise.FrontendDays.Bot.Api.Repositories.Contracts
{
    public interface ICategoryRepository : IDbRepository<Category>
    {
        Task<Category> GetUserCategory(int userId);
    }
}
