using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers.Contracts;
using Materialise.FrontendDays.Bot.Api.Properties;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class DenyStatsCommand : ICommand
    {
        private readonly ILogger<DenyStatsCommand> _logger;
        private readonly IMessageSender _messageSender;

        public DenyStatsCommand(ILogger<DenyStatsCommand> logger, IMessageSender messageSender)
        {
            _logger = logger;
            _messageSender = messageSender;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;

            _logger.LogDebug($"User {userId} tries to request stats, but he haven't already played");

            await _messageSender.SendTo(userId, Resources.StatsDenied);
        }
    }
}
