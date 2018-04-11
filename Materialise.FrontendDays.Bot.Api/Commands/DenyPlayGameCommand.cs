using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers.Contracts;
using Materialise.FrontendDays.Bot.Api.Properties;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class DenyPlayGameCommand : ICommand
    {
        private readonly ILogger<DenyPlayGameCommand> _logger;
        private readonly IMessageSender _messageSender;

        public DenyPlayGameCommand(ILogger<DenyPlayGameCommand> logger, IMessageSender messageSender)
        {
            _logger = logger;
            _messageSender = messageSender;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;

            _logger.LogDebug($"User {userId} tries to play, but he already played");

            await _messageSender.SendTo(userId, Resources.AlreadyPlayed);
        }
    }
}
