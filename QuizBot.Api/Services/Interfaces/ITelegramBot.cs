using System.Threading.Tasks;
using Telegram.Bot;

namespace QuizBot.Api.Services.Interfaces
{
    public interface ITelegramBot
    {
        Task<ITelegramBotClient> InitializeAsync();
    }
}