using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using User = Materialise.FrontendDays.Bot.Api.Models.User;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class ResponseEmailCommand : ICommand
    {
        private readonly IDbRepository<User> _usersRepository;
        private readonly ILogger<PlayGameCommand> _logger;
        private readonly PlayGameCommand _playGameCommand;

        public ResponseEmailCommand(IDbRepository<User> usersRepository, ILogger<PlayGameCommand> logger, PlayGameCommand playGameCommand)
        {
            _usersRepository = usersRepository;
            _logger = logger;
            _playGameCommand = playGameCommand;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = (await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id))
                .First();

            user.Email = update.Message.Text;
            user.UserStatus = UserStatus.ReadyForPlay;

            _logger.LogDebug($"User {user.Id} setup e-mail: {user.Email}");

            await _usersRepository.UpdateAsync(user);

            await _playGameCommand.ExecuteAsync(update);
        }
    }
}
