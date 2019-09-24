using System;
using System.Linq;
using System.Threading.Tasks;
using QuizBot.Api.Models;
using Microsoft.Extensions.Logging;
using QuizBot.Api.Commands.Interfaces;
using QuizBot.Api.Repositories.Interfaces;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands
{
    public class AnswerCommand : ICommand
    {
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IDbRepository<Answer> _answerRepository;
        private readonly NextQuestionCommand _nextQuestionCommand;
        private readonly ILogger<AnswerCommand> _logger;

        public AnswerCommand(IUserAnswerRepository userAnswerRepository, IDbRepository<Answer> answerRepository,
            NextQuestionCommand nextQuestionCommand, ILogger<AnswerCommand> logger)
        {
            _userAnswerRepository = userAnswerRepository;
            _answerRepository = answerRepository;
            _nextQuestionCommand = nextQuestionCommand;
            _logger = logger;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;
            var answer = update.Message.Text.Trim();

            _logger.LogDebug($"User {userId} answers {answer}");

            var userAnswer = (await _userAnswerRepository.FindAsync(x => x.UserId == userId && x.Answer.IsStub))
                .First();

            var realAnswer = (await _answerRepository.FindAsync(
                    x => x.Text.Equals(answer, StringComparison.InvariantCultureIgnoreCase)))
                .First();

            userAnswer.AnswerId = realAnswer.Id;

            _logger.LogDebug($"User answers {realAnswer.Text}");

            await _userAnswerRepository.UpdateAsync(userAnswer);

            await _nextQuestionCommand.ExecuteAsync(update);
        }
    }
}
