using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Builders;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class NextQuestionCommand : ICommand
    {
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IDbRepository<Question> _questionRepository;
        private readonly TelegramBotClient _botClient;
        private readonly GameFinishedCommand _gameFinishedCommand;
        private readonly ILogger<NextQuestionCommand> _logger;
        private readonly IKeyboardBuilder _builder;

        public NextQuestionCommand(TelegramBotClient botClient, IDbRepository<Question> questionRepository, 
            IUserAnswerRepository userAnswerRepository, GameFinishedCommand gameFinishedCommand, 
            ILogger<NextQuestionCommand> logger, IKeyboardBuilder builder)
        {
            _botClient = botClient;
            _questionRepository = questionRepository;
            _userAnswerRepository = userAnswerRepository;
            _gameFinishedCommand = gameFinishedCommand;
            _logger = logger;
            _builder = builder;
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

                foreach (var answer in question.PossibleAnswers)
                {
                    _builder.AddButtonsRow(answer.Text);
                }

                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, question.Text, false, false, 0, 
                    _builder.Build());
            }
            else
            {
                await _gameFinishedCommand.ExecuteAsync(update);
            }
        }
    }
}
