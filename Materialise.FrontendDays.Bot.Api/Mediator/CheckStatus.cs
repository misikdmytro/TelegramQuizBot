using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Materialise.FrontendDays.Bot.Api.Mediator
{
    public class CheckStatusRequest : IRequest<string>
    {
    }

    public class CheckStatusHandler : IRequestHandler<CheckStatusRequest, string>
    {
        private readonly ILogger<CheckStatusHandler> _logger;

        public CheckStatusHandler(ILogger<CheckStatusHandler> logger)
        {
            _logger = logger;
        }

        public Task<string> Handle(CheckStatusRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Checking status...");
            _logger.LogDebug("All OK");
            return Task.FromResult("All OK");
        }
    }
}
