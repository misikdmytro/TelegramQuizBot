using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands.Interfaces
{
    public interface ICommandsFactory
    {
        Task<ICommand> ResolveAsync(Update update);
    }
}