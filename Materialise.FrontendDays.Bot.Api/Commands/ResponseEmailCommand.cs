using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Validators;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Materialise.FrontendDays.Bot.Api.Models.User;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class ResponseEmailCommand : ICommand
    {
        private readonly IDbRepository<User> _usersRepository;
        private readonly ILogger<PlayGameCommand> _logger;
        private readonly PlayGameCommand _playGameCommand;
        private readonly EmailValidator _validator;
        private readonly TelegramBotClient _botClient;

        public ResponseEmailCommand(IDbRepository<User> usersRepository, ILogger<PlayGameCommand> logger,
            PlayGameCommand playGameCommand, EmailValidator validator, TelegramBotClient botClient)
        {
            _usersRepository = usersRepository;
            _logger = logger;
            _playGameCommand = playGameCommand;
            _validator = validator;
            _botClient = botClient;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = (await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id))
                .First();

            var email = update.Message.Text;

            if (!await _validator.IsValid(email))
            {
                _logger.LogDebug($"User {user.Id} setup sends incorrect e-mail: {email}");

                await _botClient.SendTextMessageAsync(update.Message.Chat.Id,
                    "Please, provide e-mail in correct format");

                return;
            }

            user.Email = email;
            user.UserStatus = UserStatus.ReadyForPlay;

            _logger.LogDebug($"User {user.Id} setup e-mail: {user.Email}");

            await _usersRepository.UpdateAsync(user);

            await _playGameCommand.ExecuteAsync(update);
        }
    }
}
