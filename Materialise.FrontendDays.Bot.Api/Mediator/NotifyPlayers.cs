using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Controllers;
using Materialise.FrontendDays.Bot.Api.Helpers;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Resources;
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
        private readonly MessageSender _messageSender;
        private readonly Localization _localization;

        public NotifyPlayersHandler(Localization localization, ILogger<AdminController> logger, 
            MessageSender messageSender, IDbRepository<User> userRepository)
        {
            _localization = localization;
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
                    ? _messageSender.SendTo(user.Id, _localization["winner"])
                    : _messageSender.SendTo(user.Id, _localization["loser"]));
            }

            await Task.WhenAll(tasks);
        }
    }
}
