using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands.Predicates
{
    public class RequestEmailPredicate : ICommandPredicate
    {
        private readonly IDbRepository<Models.User> _usersRepository;

        public RequestEmailPredicate(IDbRepository<Models.User> usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public bool IsDefault => false;

        public async Task<bool> IsThisCommand(Update update)
        {
            var user = (await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id))
                .FirstOrDefault();

            return update.Message.Text.Equals("/play") && user?.UserStatus == UserStatus.NewUser;
        }
    }
}
