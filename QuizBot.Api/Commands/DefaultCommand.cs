using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QuizBot.Api.Commands.Interfaces;
using QuizBot.Api.Services.Interfaces;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands
{
    public class DefaultCommand : ICommand
    {
        private readonly ILogger<DefaultCommand> _logger;
        private readonly IUserRegistrationService _registrationService;

        public DefaultCommand(ILogger<DefaultCommand> logger, IUserRegistrationService registrationService)
        {
            _logger = logger;
            _registrationService = registrationService;
        }

        public async Task ExecuteAsync(Update update)
        {
            await _registrationService.RegisterIfNotExists(update);

            var messageText = update.Message.Text;
            var userId = update.Message.From.Id;

            _logger.LogDebug($"User {userId} sends {messageText}");
        }
    }
}
