using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace QuizBot.Api.Mediator
{
    public class CheckStatusRequest : IRequest<string>
    {
    }

    public class CheckStatusHandler : IRequestHandler<CheckStatusRequest, string>
    {
        private readonly ILogger<CheckStatusHandler> _logger;
        private readonly ITelegramBotClient _botClient; //just to connect to Telegram API

        public CheckStatusHandler(ILogger<CheckStatusHandler> logger, ITelegramBotClient botClient)
        {
            _logger = logger;
            _botClient = botClient;
        }

        public Task<string> Handle(CheckStatusRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Checking status...");
            _logger.LogDebug("All OK");
            return Task.FromResult("All OK");
        }
    }
}
