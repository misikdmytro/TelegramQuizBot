using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Extensions;
using Materialise.FrontendDays.Bot.Api.Filters;
using Materialise.FrontendDays.Bot.Api.Helpers;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Materialise.FrontendDays.Bot.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/admin")]
    [BasicAuthenticationFilter]
    public class AdminController : Controller
    {
        private readonly Question[] _questions;
        private readonly IDbRepository<Question> _questionRepository;
        private readonly IDbRepository<Answer> _answersRepository;
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IDbRepository<User> _userRepository;
        private readonly MessageSender _messageSender;
        private readonly Localization _localization;
        private readonly ILogger<AdminController> _logger;

        public AdminController(Question[] questions, IDbRepository<Question> questionRepository,
            IDbRepository<Answer> answersRepository, IUserAnswerRepository userAnswerRepository,
            IDbRepository<User> userRepository, Localization localization,
            ILogger<AdminController> logger, MessageSender messageSender)
        {
            _questions = questions;
            _questionRepository = questionRepository;
            _answersRepository = answersRepository;
            _userAnswerRepository = userAnswerRepository;
            _userRepository = userRepository;
            _localization = localization;
            _logger = logger;
            _messageSender = messageSender;
        }

        [HttpGet]
        [Route("status")]
        public IActionResult Status()
        {
            _logger.LogDebug("Checking status...");
            _logger.LogDebug("All OK");
            return Ok("All OK");
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> ClearDatabase()
        {
            _logger.LogDebug("Updating DB...");

            await _answersRepository.ClearAsync();
            await _questionRepository.ClearAsync();
            await _userAnswerRepository.ClearAsync();

            foreach (var question in _questions)
            {
                await _questionRepository.AddAsync(question);
            }

            var users = await _userRepository.FindAsync(x => true);

            foreach (var user in users)
            {
                user.UserStatus = user.NeedEmailInfo() 
                    ? user.UserStatus 
                    : UserStatus.ReadyForPlay;

                user.IsWinner = false;

                await _userRepository.UpdateAsync(user);
            }

            _logger.LogDebug("DB updated");

            return Ok();
        }

        [HttpGet]
        [Route("winner")]
        public async Task<IActionResult> Winner()
        {
            _logger.LogDebug("Get winner...");

            var winner = (await _userRepository.FindAsync(x => x.IsWinner)).FirstOrDefault();

            if (winner != null)
            {
                _logger.LogDebug($"Winner is {winner.Id}");
                return Ok(winner);
            }

            var possibleWinners = await _userRepository.FindAsync(u => u.UserStatus == UserStatus.Answered);

            if (!possibleWinners.Any())
            {
                _logger.LogDebug("No winners");
                return Ok();
            }

            var rand = new Random((int)DateTime.Now.Ticks);
            var index = rand.Next(0, possibleWinners.Length);
            winner = possibleWinners.ElementAt(index);

            winner.IsWinner = true;

            await _userRepository.UpdateAsync(winner);

            _logger.LogDebug($"Winner is {winner.Id}");
            return Ok(winner);
        }

        [HttpDelete]
        [Route("winner")]
        public async Task<IActionResult> RemoveWinner()
        {
            _logger.LogDebug("Delete winners...");

            var winner = (await _userRepository.FindAsync(x => x.IsWinner)).FirstOrDefault();

            if (winner != null)
            {
                _logger.LogDebug($"Current winner is {winner.Id}");

                winner.IsWinner = false;
                await _userRepository.UpdateAsync(winner);

                return Ok(winner);
            }

            return Ok();
        }

        [HttpPost]
        [Route("notify")]
        public async Task<IActionResult> Notify()
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

            return Ok();
        }
    }
}