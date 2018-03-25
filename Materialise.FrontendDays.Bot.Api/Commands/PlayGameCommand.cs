using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Extensions;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Resources;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
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
        private readonly IUserRegistrationService _registrationService;

        public PlayGameCommand(TelegramBotClient botClient, NextQuestionCommand nextQuestionCommand,
            Localization localization, ILogger<PlayGameCommand> logger, 
            IDbRepository<Models.User> usersRepository, RequestEmailCommand requestEmailCommand, 
            IUserRegistrationService registrationService)
        {
            _botClient = botClient;
            _nextQuestionCommand = nextQuestionCommand;
            _localization = localization;
            _logger = logger;
            _usersRepository = usersRepository;
            _requestEmailCommand = requestEmailCommand;
            _registrationService = registrationService;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = await _registrationService.RegisterIfNotExists(update);

            _logger.LogDebug($"User {update.Message.From.Id} tries to start game");

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
