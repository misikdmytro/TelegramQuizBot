using System.Threading.Tasks;
using QuizBot.Api.Properties;
using Microsoft.Extensions.Logging;
using QuizBot.Api.Commands.Interfaces;
using QuizBot.Api.Helpers.Interfaces;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands
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
