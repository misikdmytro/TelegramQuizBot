﻿using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Resources;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class GameFinishedCommand : ICommand
    {
        private readonly IDbRepository<UserAnswer> _userAnswerRepository;
        private readonly Localization _localization;
        private readonly ILogger<GameFinishedCommand> _logger;
        private readonly IDbRepository<Models.User> _useRepository;
        private readonly MessageSender _messageSender;

        public GameFinishedCommand(IDbRepository<UserAnswer> userAnswerRepository,
            Localization localization, ILogger<GameFinishedCommand> logger,
            IDbRepository<Models.User> useRepository, MessageSender messageSender)
        {
            _userAnswerRepository = userAnswerRepository;
            _localization = localization;
            _logger = logger;
            _useRepository = useRepository;
            _messageSender = messageSender;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;

            var user = (await _useRepository.FindAsync(x => x.Id == userId))
                .First();

            await _messageSender.SendTo(userId, _localization["allCorrectResponse"]);

            user.UserStatus = UserStatus.Answered;

            await _useRepository.UpdateAsync(user);
        }
    }
}
