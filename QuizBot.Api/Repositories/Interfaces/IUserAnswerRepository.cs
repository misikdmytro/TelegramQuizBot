using System.Threading.Tasks;
using QuizBot.Api.Models;

namespace QuizBot.Api.Repositories.Interfaces
{
    public interface IUserAnswerRepository : IDbRepository<UserAnswer>
    {
        Task<Question[]> GetAnsweredQuestions(int userId);
    }
}
