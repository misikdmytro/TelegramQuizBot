using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;

namespace Materialise.FrontendDays.Bot.Api.Repositories.Contracts
{
    public interface IUserAnswerRepository : IDbRepository<UserAnswer>
    {
        Task<Question[]> GetAnsweredQuestions(int userId);
    }
}
