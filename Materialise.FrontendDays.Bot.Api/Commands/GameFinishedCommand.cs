using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Resources;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class GameFinishedCommand : ICommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly Localization _localization;
        private readonly ILogger<GameFinishedCommand> _logger;
        private readonly IDbRepository<Models.User> _useRepository;

        public GameFinishedCommand(TelegramBotClient botClient, IUserAnswerRepository userAnswerRepository,
            Localization localization, ILogger<GameFinishedCommand> logger, IDbRepository<Models.User> useRepository)
        {
            _botClient = botClient;
            _userAnswerRepository = userAnswerRepository;
            _localization = localization;
            _logger = logger;
            _useRepository = useRepository;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;

            var isAllCorrect = (await _userAnswerRepository.FindAsync(x => x.UserId == userId))
                .All(x => x.IsCorrect == true);

            var user = (await _useRepository.FindAsync(x => x.Id == userId))
                .First();

            if (isAllCorrect)
            {
                _logger.LogDebug($"User {userId} gives correct answers");
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, _localization["allCorrectResponse"],
                    false, false, 0, new ReplyKeyboardHide());

                user.UserStatus = UserStatus.Answered;
            }
            else
            {
                _logger.LogDebug($"User {userId} gives some(all) incorrect answers");
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, _localization["notAllCorrectResponse"],
                    false, false, 0, new ReplyKeyboardHide());

                user.UserStatus = UserStatus.Failed;
            }

            await _useRepository.UpdateAsync(user);
        }
    }
}
