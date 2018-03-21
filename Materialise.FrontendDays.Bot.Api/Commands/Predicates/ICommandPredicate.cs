using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands.Predicates
{
    public interface ICommandPredicate
    {
        Task<bool> IsThat(Update update);
    }
}
