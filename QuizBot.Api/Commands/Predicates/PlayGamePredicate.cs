using System.Linq;
using System.Threading.Tasks;
using QuizBot.Api.Commands.Predicates.Interfaces;
using QuizBot.Api.Models;
using QuizBot.Api.Repositories.Interfaces;
using Telegram.Bot.Types;
using User = QuizBot.Api.Models.User;

namespace QuizBot.Api.Commands.Predicates
{
    public class PlayGamePredicate : ICommandPredicate
    {
        private readonly IDbRepository<User> _usersRepository;

        public PlayGamePredicate(IDbRepository<User> usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<bool> IsThisCommand(Update update)
        {
            var user = (await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id))
                .FirstOrDefault();

            return update.Message.Text.Equals("/play") && user?.UserStatus == UserStatus.NewUser;
        }
    }
}
