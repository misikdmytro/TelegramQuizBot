using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public delegate Task<ICommand> CommandFactoryMethod(Update update);

    public class CommandsFactory : ICommandsFactory
    {
        private readonly CommandFactoryMethod _commandFactoryMethod;

        public CommandsFactory(CommandFactoryMethod commandFactoryMethod)
        {
            _commandFactoryMethod = commandFactoryMethod;
        }

        public async Task<ICommand> ResolveAsync(Update update)
        {
            return await _commandFactoryMethod(update);
        }
    }
}
