using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Materialise.FrontendDays.Bot.Api.Commands.Predicates;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class CommandsStrategy : ICommandsStrategy
    {
        private readonly IComponentContext _resolver;
        private readonly IEnumerable<KeyValuePair<ICommandPredicate, Type>> _commands;

        public CommandsStrategy(IComponentContext resolver, IEnumerable<KeyValuePair<ICommandPredicate, Type>> commands)
        {
            _resolver = resolver;
            _commands = commands;
        }

        public async Task<ICommand> ResolveAsync(Update update)
        {
            foreach (var nameTypePair in _commands)
            {
                if (await nameTypePair.Key.IsThisCommand(update))
                {
                    return (ICommand)_resolver.Resolve(nameTypePair.Value);
                }
            }

            return (ICommand)_resolver.Resolve(_commands
                .First(x => x.Key.IsDefault).Value);
        }
    }
}
