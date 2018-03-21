using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;

namespace Materialise.FrontendDays.Bot.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IDbRepository<Question> _questionRepository;

        public UserService(IUserAnswerRepository userAnswerRepository, 
            IDbRepository<Question> questionRepository)
        {
            _userAnswerRepository = userAnswerRepository;
            _questionRepository = questionRepository;
        }

        public async Task<User[]> GetPossibleWinnersAsync()
        {
            var questionsNumber = (await _questionRepository.FindAsync(x => true)).Length;
            var correctAnswers = await _userAnswerRepository.GetCorrectAnswers();

            return correctAnswers.GroupBy(g => g.User)
                .Where(x => x.Count() == questionsNumber)
                .Select(x => x.Key)
                .ToArray();
        }
    }
}
