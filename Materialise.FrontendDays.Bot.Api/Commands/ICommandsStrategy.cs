using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public interface ICommandsStrategy
    {
        Task<ICommand> ResolveAsync(Update update);
    }
}