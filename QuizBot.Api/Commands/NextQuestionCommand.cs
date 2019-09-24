using System.Linq;
using System.Threading.Tasks;
using QuizBot.Api.Models;
using Microsoft.Extensions.Logging;
using QuizBot.Api.Commands.Interfaces;
using QuizBot.Api.Helpers.Interfaces;
using QuizBot.Api.Repositories.Interfaces;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands
{
    public class NextQuestionCommand : ICommand
    {
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IDbRepository<Question> _questionRepository;
        private readonly GameFinishedCommand _gameFinishedCommand;
        private readonly ILogger<NextQuestionCommand> _logger;
        private readonly IMessageSender _messageSender;
        private readonly IDbRepository<Answer> _answerRepository;

        public NextQuestionCommand(IDbRepository<Question> questionRepository, 
            IUserAnswerRepository userAnswerRepository, GameFinishedCommand gameFinishedCommand, 
            ILogger<NextQuestionCommand> logger, IMessageSender messageSender, 
            IDbRepository<Answer> answerRepository)
        {
            _questionRepository = questionRepository;
            _userAnswerRepository = userAnswerRepository;
            _gameFinishedCommand = gameFinishedCommand;
            _logger = logger;
            _messageSender = messageSender;
            _answerRepository = answerRepository;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;

            var answeredQuestions = await _userAnswerRepository.GetAnsweredQuestions(userId);

            var question = (await _questionRepository.FindAsync(x => answeredQuestions.All(a => a.Id != x.Id)))
                .FirstOrDefault();

            if (question != null)
            {
                var stub = (await _answerRepository.FindAsync(x => x.IsStub && x.QuestionId == question.Id))
                    .First();

                var userAnswer = new UserAnswer
                {
                    UserId = userId,
                    AnswerId = stub.Id
                };

                _logger.LogDebug($"User {userId} receives question {question.Id}");

                await _userAnswerRepository.AddAsync(userAnswer);

                await _messageSender.SendTo(userId, question.Text, question.PossibleAnswers.Where(x => !x.IsStub)
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
