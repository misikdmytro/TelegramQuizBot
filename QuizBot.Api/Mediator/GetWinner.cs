using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QuizBot.Api.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using QuizBot.Api.Repositories.Interfaces;

namespace QuizBot.Api.Mediator
{
    public class GetWinnerRequest : IRequest<User>
    {
    }

    public class GetWinnerHandler : IRequestHandler<GetWinnerRequest, User>
    {
        private readonly IDbRepository<User> _userRepository;
        private readonly ILogger<GetWinnerHandler> _logger;

        public GetWinnerHandler(ILogger<GetWinnerHandler> logger, IDbRepository<User> userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<User> Handle(GetWinnerRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Get winner...");

            var winner = (await _userRepository.FindAsync(x => x.IsWinner)).FirstOrDefault();

            if (winner != null)
            {
                _logger.LogDebug($"Winner is {winner.Id}");
                return winner;
            }

            var possibleWinners = await _userRepository.FindAsync(u => u.UserStatus == UserStatus.Answered);

            if (!possibleWinners.Any())
            {
                _logger.LogDebug("No winners");
                return null;
            }

            var rand = new Random((int)DateTime.Now.Ticks);
            var index = rand.Next(0, possibleWinners.Length);
            winner = possibleWinners.ElementAt(index);

            winner.IsWinner = true;

            await _userRepository.UpdateAsync(winner);

            _logger.LogDebug($"Winner is {winner.Id}");

            return winner;
        }
    }
}
