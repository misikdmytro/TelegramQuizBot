using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands.Predicates
{
    public class StartPredicate : ICommandPredicate
    {
        public bool IsDefault => false;

        public Task<bool> IsThisCommand(Update update)
        {
            return Task.FromResult(update.Message.Text.Equals("/start"));
        }
    }
}
