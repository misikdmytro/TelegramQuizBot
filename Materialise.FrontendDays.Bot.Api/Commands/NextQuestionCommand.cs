using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class NextQuestionCommand : ICommand
    {
        private readonly IDbRepository<UserAnswer> _userAnswerRepository;
        private readonly IDbRepository<Question> _questionRepository;
        private readonly GameFinishedCommand _gameFinishedCommand;
        private readonly ILogger<NextQuestionCommand> _logger;
        private readonly MessageSender _messageSender;

        public NextQuestionCommand(IDbRepository<Question> questionRepository, 
            IDbRepository<UserAnswer> userAnswerRepository, GameFinishedCommand gameFinishedCommand, 
            ILogger<NextQuestionCommand> logger, MessageSender messageSender)
        {
            _questionRepository = questionRepository;
            _userAnswerRepository = userAnswerRepository;
            _gameFinishedCommand = gameFinishedCommand;
            _logger = logger;
            _messageSender = messageSender;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;

            var userAnswers = await _userAnswerRepository.FindAsync(x => x.UserId == userId);
            var question = (await _questionRepository.FindAsync(x => userAnswers.All(a => a.QuestionId != x.Id)))
                .FirstOrDefault();

            if (question != null)
            {
                var userAnswer = new UserAnswer
                {
                    QuestionId = question.Id,
                    UserId = userId
                };

                _logger.LogDebug($"User {userId} receives question {question.Id}");

                await _userAnswerRepository.AddAsync(userAnswer);

                await _messageSender.SendTo(userId, question.Text, question.PossibleAnswers
                    .Select(x => x.Text)
                    .ToArray());
            }
            else
            {
                await _gameFinishedCommand.ExecuteAsync(update);
            }
        }
    }
}
