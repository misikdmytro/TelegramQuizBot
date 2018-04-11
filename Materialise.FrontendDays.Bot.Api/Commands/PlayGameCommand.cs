using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers.Contracts;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Properties;
using Materialise.FrontendDays.Bot.Api.Repositories.Contracts;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class PlayGameCommand : ICommand
    {
        private readonly NextQuestionCommand _nextQuestionCommand;
        private readonly ILogger<PlayGameCommand> _logger;
        private readonly IDbRepository<Models.User> _usersRepository;
        private readonly IUserRegistrationService _registrationService;
        private readonly IMessageSender _messageSender;

        public PlayGameCommand(NextQuestionCommand nextQuestionCommand,
            ILogger<PlayGameCommand> logger,
            IDbRepository<Models.User> usersRepository,
            IUserRegistrationService registrationService, IMessageSender messageSender)
        {
            _nextQuestionCommand = nextQuestionCommand;
            _logger = logger;
            _usersRepository = usersRepository;
            _registrationService = registrationService;
            _messageSender = messageSender;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = await _registrationService.RegisterIfNotExists(update);

            _logger.LogDebug($"User {update.Message.From.Id} tries to start game");

            user.UserStatus = UserStatus.Player;
            await _usersRepository.UpdateAsync(user);

            await _messageSender.SendTo(user.Id, Resources.GameStarted);
            await _nextQuestionCommand.ExecuteAsync(update);
        }
    }
}
