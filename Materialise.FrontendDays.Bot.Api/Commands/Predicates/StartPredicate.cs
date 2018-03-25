using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands.Predicates
{
    public class StartPredicate : ICommandPredicate
    {
        private readonly IDbRepository<Models.User> _usersRepository;

        public StartPredicate(IDbRepository<Models.User> usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public bool IsDefault => false;

        public async Task<bool> IsThisCommand(Update update)
        {
            var user = (await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id))
                .FirstOrDefault();

            return user == null && update.Message.Text.Equals("/start");
        }
    }
}
