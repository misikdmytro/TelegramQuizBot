using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Materialise.FrontendDays.Bot.Api.Models.User;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class CheckEmailCommand : ICommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly IDbRepository<User> _usersRepository;
        private readonly PlayGameCommand _playGameCommand;
        private readonly ILogger<CheckEmailCommand> _logger;

        public CheckEmailCommand(TelegramBotClient botClient, IDbRepository<User> usersRepository,
            PlayGameCommand playGameCommand, ILogger<CheckEmailCommand> logger)
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
            _playGameCommand = playGameCommand;
            _logger = logger;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = (await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id))
                .First();

            if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogDebug($"Request user's {user.Id} e-mail");
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Плиз ду, оставь мыло");
            }
            else
            {
                await _playGameCommand.ExecuteAsync(update);
            }
        }
    }
}
