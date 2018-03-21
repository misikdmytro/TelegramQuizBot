using System.Threading.Tasks;
using Telegram.Bot;

namespace Materialise.FrontendDays.Bot.Api.Services.Contracts
{
    public interface ITelegramBot
    {
        Task<TelegramBotClient> InitializeAsync();
    }
}