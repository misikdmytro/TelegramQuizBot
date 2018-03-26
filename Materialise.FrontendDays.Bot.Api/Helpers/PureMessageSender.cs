using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = Materialise.FrontendDays.Bot.Api.Models.User;

namespace Materialise.FrontendDays.Bot.Api.Builders
{
    public class PureMessageSender
    {
        private readonly TelegramBotClient _botClient;
        private readonly ILogger<PureMessageSender> _logger;
        private readonly IDbRepository<User> _userRepository;

        public PureMessageSender(TelegramBotClient botClient, ILogger<PureMessageSender> logger, 
            IDbRepository<User> userRepository)
        {
            _botClient = botClient;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task SendTo(int userId, string message)
        {
            var user = (await _userRepository.FindAsync(x => x.Id == userId))
                .First();

            _logger.LogDebug($"Send next message to user {userId}: '{message}'");

            await _botClient.SendTextMessageAsync(user.ChatId, message, false, false, 0, new ReplyKeyboardHide());
        }

        public async Task SendTo(int userId, string message, params string[] options)
        {
            var user = (await _userRepository.FindAsync(x => x.Id == userId))
                .First();

            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = options
                    .Select(o => new[] { new KeyboardButton(o) })
                    .ToArray()
            };

            _logger.LogDebug($"Send next keyboard to user {userId}: '{message}'");

            await _botClient.SendTextMessageAsync(user.ChatId, message, false, false, 0, keyboard);
        }
    }
}
