using System.Threading.Tasks;
using QuizBot.Api.Properties;
using Microsoft.Extensions.Logging;
using QuizBot.Api.Commands.Interfaces;
using QuizBot.Api.Helpers.Interfaces;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands
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
