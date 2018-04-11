using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Commands.Predicates.Contracts;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class CommandsFactory : ICommandsFactory
    {
        private const string PredicateSuffix = "Predicate";
        private const string CommandSuffix = "Command";

        private readonly IComponentContext _resolver;

        public CommandsFactory(IComponentContext resolver)
        {
            _resolver = resolver;
        }

        public async Task<ICommand> ResolveAsync(Update update)
        {
            var predicateTypes = GetType().Assembly.GetTypes()
                .Where(t => typeof(ICommandPredicate).IsAssignableFrom(t))
                .ToArray();

            foreach (var type in predicateTypes)
            {
                var command = (ICommandPredicate)_resolver.Resolve(type);

                if (await command.IsThisCommand(update))
                {
                    var rootName = type.Name.Remove(type.Name.IndexOf(PredicateSuffix,
                        StringComparison.Ordinal));

                    var commandName = rootName + CommandSuffix;

                    var commandType = GetType().Assembly
                        .GetTypes()
                        .Where(t => typeof(ICommand).IsAssignableFrom(t))
                        .FirstOrDefault(t => t.Name.Equals(commandName));

                    return (ICommand)_resolver.Resolve(commandType);
                }
            }

            return (ICommand)_resolver.Resolve(typeof(DefaultCommand));
        }
    }
}
