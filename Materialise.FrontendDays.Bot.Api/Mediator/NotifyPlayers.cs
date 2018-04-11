using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Controllers;
using Materialise.FrontendDays.Bot.Api.Helpers.Contracts;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Properties;
using Materialise.FrontendDays.Bot.Api.Repositories.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Materialise.FrontendDays.Bot.Api.Mediator
{
    public class NotifyPlayersRequest : IRequest
    {
    }

    public class NotifyPlayersHandler : IRequestHandler<NotifyPlayersRequest>
    {
        private readonly IDbRepository<User> _userRepository;
        private readonly ILogger<AdminController> _logger;
        private readonly IMessageSender _messageSender;

        public NotifyPlayersHandler(ILogger<AdminController> logger, 
            IMessageSender messageSender, IDbRepository<User> userRepository)
        {
            _logger = logger;
            _messageSender = messageSender;
            _userRepository = userRepository;
        }

        public async Task Handle(NotifyPlayersRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start sending notifications...");

            var tasks = new List<Task>();

            foreach (var user in await _userRepository.FindAsync(x => true))
            {
                tasks.Add(user.IsWinner
                    ? _messageSender.SendTo(user.Id, Resources.Winner)
                    : _messageSender.SendTo(user.Id, Resources.Loser));
            }

            await Task.WhenAll(tasks);
        }
    }
}
