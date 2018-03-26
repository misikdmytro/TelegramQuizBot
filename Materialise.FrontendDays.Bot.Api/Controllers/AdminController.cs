using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Filters;
using Materialise.FrontendDays.Bot.Api.Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Materialise.FrontendDays.Bot.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/admin")]
    [BasicAuthenticationFilter]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> Status()
        {
            var result = await _mediator.Send(new CheckStatusRequest());
            return Ok(result);
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> ClearDatabase()
        {
            await _mediator.Send(new UpdateDatabaseRequest());
            return Ok();
        }

        [HttpGet]
        [Route("winner")]
        public async Task<IActionResult> Winner()
        {
            var winner = await _mediator.Send(new GetWinnerRequest());
            return Ok(winner);
        }

        [HttpDelete]
        [Route("winner")]
        public async Task<IActionResult> RemoveWinner()
        {
            var winner = await _mediator.Send(new RemoveWinnerRequest());
            return Ok(winner);
        }

        [HttpPost]
        [Route("notify")]
        public async Task<IActionResult> Notify()
        {
            await _mediator.Send(new NotifyPlayersRequest());
            return Ok();
        }
    }
}