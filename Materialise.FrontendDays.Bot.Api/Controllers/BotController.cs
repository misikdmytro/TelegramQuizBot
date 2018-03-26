using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Materialise.FrontendDays.Bot.Api.Controllers
{
    [Route("api/[controller]")]
    public class BotController : Controller
    {
        private readonly IMediator _mediator;

        public BotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _mediator.Send(new UserUpdateRequest(update));
            return Ok();
        }
    }
}
