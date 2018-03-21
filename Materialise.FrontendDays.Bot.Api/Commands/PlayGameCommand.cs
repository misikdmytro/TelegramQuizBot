using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Extensions;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Resources;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class PlayGameCommand : ICommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly NextQuestionCommand _nextQuestionCommand;
        private readonly Localization _localization;
        private readonly ILogger<PlayGameCommand> _logger;
        private readonly IDbRepository<Models.User> _usersRepository;
        private readonly RequestEmailCommand _requestEmailCommand;

        public PlayGameCommand(TelegramBotClient botClient, NextQuestionCommand nextQuestionCommand,
            Localization localization, ILogger<PlayGameCommand> logger, IUserAnswerRepository userAnswerRepository, 
            IDbRepository<Models.User> usersRepository, RequestEmailCommand requestEmailCommand)
        {
            _botClient = botClient;
            _nextQuestionCommand = nextQuestionCommand;
            _localization = localization;
            _logger = logger;
            _usersRepository = usersRepository;
            _requestEmailCommand = requestEmailCommand;
        }

        public async Task ExecuteAsync(Update update)
        {
            if (!(await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id)).Any())
            {
                var newUser = new Models.User
                {
                    LastName = update.Message.From.LastName,
                    Id = update.Message.From.Id,
                    FirstName = update.Message.From.FirstName,
                    Username = update.Message.From.Username,
                    ChatId = update.Message.Chat.Id,
                };

                _logger.LogDebug($"New user registered: {newUser.FirstName} {newUser.LastName}");
                await _usersRepository.AddAsync(newUser);
            }

            _logger.LogDebug($"User {update.Message.From.Id} starts game");

            var user = (await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id))
                .First();

            if (user.NeedEmailInfo())
            {
                await _requestEmailCommand.ExecuteAsync(update);
            }
            else if (user.HasPlayed())
            {
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, _localization["gamePlayed"]);
            }
            else
            {
                user.UserStatus = UserStatus.Player;
                await _usersRepository.UpdateAsync(user);

                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, _localization["gameStarted"]);
                await _nextQuestionCommand.ExecuteAsync(update);
            }
        }
    }
}
