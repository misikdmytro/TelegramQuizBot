using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Resources;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Materialise.FrontendDays.Bot.Api.Models.User;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class StartCommand : ICommand
    {
        private readonly IDbRepository<User> _usersRepository;
        private readonly TelegramBotClient _botClient;
        private readonly Localization _localization;
        private readonly ILogger<StartCommand> _logger;

        public StartCommand(IDbRepository<User> usersRepository, TelegramBotClient botClient, Localization localization, 
            ILogger<StartCommand> logger)
        {
            _usersRepository = usersRepository;
            _botClient = botClient;
            _localization = localization;
            _logger = logger;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = new User
            {
                LastName = update.Message.From.LastName,
                Id = update.Message.From.Id,
                FirstName = update.Message.From.FirstName,
                Username = update.Message.From.Username,
                ChatId = update.Message.Chat.Id,
            };

            if (!(await _usersRepository.FindAsync(x => x.Id == user.Id)).Any())
            {
                _logger.LogDebug($"New user registered: {user.FirstName} {user.LastName}");
                await _usersRepository.AddAsync(user);
            }

            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, 
                string.Format(_localization["helloFormat"], user.FirstName, user.LastName));
        }
    }
}
