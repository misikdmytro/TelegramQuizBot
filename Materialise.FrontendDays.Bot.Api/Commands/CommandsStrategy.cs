using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class CommandsStrategy : ICommandsStrategy
    {
        private readonly IComponentContext _resolver;
        private readonly IEnumerable<KeyValuePair<string, Type>> _commands;

        public CommandsStrategy(IComponentContext resolver, IEnumerable<KeyValuePair<string, Type>> commands)
        {
            _resolver = resolver;
            _commands = commands;
        }

        public ICommand Resolve(string command)
        {
            foreach (var nameTypePair in _commands)
            {
                if (nameTypePair.Key.Equals(command, StringComparison.InvariantCultureIgnoreCase))
                {
                    return (ICommand)_resolver.Resolve(nameTypePair.Value);
                }
            }

            return (ICommand)_resolver.Resolve(_commands
                .First(x => x.Key.Equals("default", StringComparison.InvariantCultureIgnoreCase)).Value);
        }
    }
}
