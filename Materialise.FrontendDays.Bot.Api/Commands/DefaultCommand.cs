using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
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
