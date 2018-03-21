using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public interface ICommand
    {
        Task ExecuteAsync(Update update);
    }
}
