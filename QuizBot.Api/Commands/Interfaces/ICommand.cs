using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands.Interfaces
{
    public interface ICommand
    {
        Task ExecuteAsync(Update update);
    }
}
