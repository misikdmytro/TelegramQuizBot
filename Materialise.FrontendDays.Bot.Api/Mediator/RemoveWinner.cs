using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Materialise.FrontendDays.Bot.Api.Mediator
{
    public class RemoveWinnerRequest : IRequest<User>
    {
    }

    public class RemoveWinnerHandler : IRequestHandler<RemoveWinnerRequest, User>
    {
        private readonly IDbRepository<User> _userRepository;
        private readonly ILogger<RemoveWinnerHandler> _logger;

        public RemoveWinnerHandler(ILogger<RemoveWinnerHandler> logger, IDbRepository<User> userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<User> Handle(RemoveWinnerRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Delete winners...");

            var winner = (await _userRepository.FindAsync(x => x.IsWinner)).FirstOrDefault();

            if (winner != null)
            {
                _logger.LogDebug($"Current winner is {winner.Id}");

                winner.IsWinner = false;
                await _userRepository.UpdateAsync(winner);

                return winner;
            }

            return null;
        }
    }
}
