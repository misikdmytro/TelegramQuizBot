using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands.Predicates.Interfaces
{
    public interface ICommandPredicate
    {
        Task<bool> IsThisCommand(Update update);
    }
}
