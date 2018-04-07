using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Materialise.FrontendDays.Bot.Api.Services
{
    public class TelegramBot : ITelegramBot
    {
        private readonly BotInfo _botInfo;
        private ITelegramBotClient _botClient;

        private readonly ILogger<TelegramBot> _logger;

        public TelegramBot(BotInfo botInfo, ILogger<TelegramBot> logger)
        {
            _botInfo = botInfo;
            _logger = logger;
        }

        public async Task<ITelegramBotClient> InitializeAsync()
        {
            if (_botClient == null)
            {
                _botClient = new TelegramBotClient(_botInfo.Token);
                await _botClient.SetWebhookAsync(_botInfo.HostUrl);

                _logger.LogInformation($"Bot with token {_botInfo.Token} initialized.");
                _logger.LogInformation($"Host - {_botInfo.HostUrl}");
            }

            return _botClient;
        }
    }
}
