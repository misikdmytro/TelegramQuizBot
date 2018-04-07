using System.Threading;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Mediator
{
    public class UserUpdateRequest : IRequest
    {
        public UserUpdateRequest(Update update)
        {
            Update = update;
        }

        public Update Update { get; }
    }

    public class UserUpdateResponse : IRequestHandler<UserUpdateRequest>
    {
        private readonly ICommandsStrategy _commandsStrategy;
        private readonly ILogger<UserUpdateResponse> _logger;

        public UserUpdateResponse(ICommandsStrategy commandsStrategy, ILogger<UserUpdateResponse> logger)
        {
            _commandsStrategy = commandsStrategy;
            _logger = logger;
        }

        public async Task Handle(UserUpdateRequest request, CancellationToken cancellationToken)
        {
            var update = request.Update;
            var userId = update?.Message?.From?.Id;

            if (update?.Message?.Text == null)
            {
                _logger.LogDebug("Incognito user sends no message");
                return;
            }

            _logger.LogDebug($"User {userId} sends next message: '{update.Message.Text}'");

            var command = await _commandsStrategy.ResolveAsync(update);
            await command.ExecuteAsync(update);
        }
    }
}
