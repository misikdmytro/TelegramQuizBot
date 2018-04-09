using System;
using System.Threading;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Materialise.FrontendDays.Bot.Api.Services
{
    public class TelegramBot : ITelegramBot
    {
        private readonly Lazy<Task<ITelegramBotClient>> _botClient;

        public TelegramBot(BotInfo botInfo, ILogger<TelegramBot> logger)
        {
            _botClient = new Lazy<Task<ITelegramBotClient>>(async () =>
            {
                var client = new TelegramBotClient(botInfo.Token);
                await client.SetWebhookAsync(botInfo.HostUrl);

                logger.LogInformation($"Bot with token {botInfo.Token} initialized.");
                logger.LogInformation($"Host - {botInfo.HostUrl}");

                return client;
            }, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public async Task<ITelegramBotClient> InitializeAsync()
        {
            return await _botClient.Value;
        }
    }
}
