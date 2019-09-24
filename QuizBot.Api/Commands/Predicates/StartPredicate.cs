using System.Threading.Tasks;
using QuizBot.Api.Commands.Predicates.Interfaces;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands.Predicates
{
    public class StartPredicate : ICommandPredicate
    {
        public Task<bool> IsThisCommand(Update update)
        {
            return Task.FromResult(update.Message.Text.Equals("/start"));
        }
    }
}
