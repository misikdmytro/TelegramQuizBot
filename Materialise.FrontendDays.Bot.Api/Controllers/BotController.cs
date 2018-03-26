using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Materialise.FrontendDays.Bot.Api.Controllers
{
    [Route("api/[controller]")]
    public class BotController : Controller
    {
        private readonly ICommandsStrategy _commandsStrategy;
        private readonly ILogger<BotController> _logger;

        public BotController(ICommandsStrategy commandsStrategy, ILogger<BotController> logger)
        {
            _commandsStrategy = commandsStrategy;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            var userId = update?.Message?.From?.Id;

            if (update?.Message?.Text == null)
            {
                _logger.LogDebug("Incognito user send no message");
                return Ok();
            }

            _logger.LogDebug($"User {userId} sends next message: '{update.Message.Text}'");
            await (await _commandsStrategy.ResolveAsync(update)).ExecuteAsync(update);

            return Ok();
        }
    }
}
