using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class DefaultCommand : ICommand
    {
        private readonly ILogger<DefaultCommand> _logger;

        public DefaultCommand(ILogger<DefaultCommand> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(Update update)
        {
            var messageText = update.Message.Text;
            var userId = update.Message.From.Id;

            _logger.LogDebug($"User {userId} sends {messageText}");

            return Task.CompletedTask;
        }
    }
}
