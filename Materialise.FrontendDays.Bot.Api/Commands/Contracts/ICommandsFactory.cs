using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands.Contracts
{
    public interface ICommandsFactory
    {
        Task<ICommand> ResolveAsync(Update update);
    }
}