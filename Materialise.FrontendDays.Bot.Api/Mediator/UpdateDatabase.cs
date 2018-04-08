using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Materialise.FrontendDays.Bot.Api.Mediator
{
    public class UpdateDatabaseRequest : IRequest
    {
    }

    public class UpdateDatabaseHandler : IRequestHandler<UpdateDatabaseRequest>
    {
        private readonly IDbRepository<Question> _questionRepository;
        private readonly IDbRepository<Answer> _answersRepository;
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly ILogger<UpdateDatabaseHandler> _logger;
        private readonly IDbRepository<User> _userRepository;
        private readonly IDbRepository<Category> _categoriesRepository;
        private readonly IDbRepository<Answer> _answerRepository;

        public UpdateDatabaseHandler(IDbRepository<Answer> answersRepository, 
            ILogger<UpdateDatabaseHandler> logger, IDbRepository<Question> questionRepository, 
            IUserAnswerRepository userAnswerRepository, IDbRepository<User> userRepository, 
            IDbRepository<Category> categoriesRepository, IDbRepository<Answer> answerRepository)
        {
            _answersRepository = answersRepository;
            _logger = logger;
            _questionRepository = questionRepository;
            _userAnswerRepository = userAnswerRepository;
            _userRepository = userRepository;
            _categoriesRepository = categoriesRepository;
            _answerRepository = answerRepository;
        }

        public async Task Handle(UpdateDatabaseRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Updating DB...");

            await _answersRepository.ClearAsync();
            await _questionRepository.ClearAsync();
            await _userAnswerRepository.ClearAsync();
            await _categoriesRepository.ClearAsync();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            foreach (var category in configuration.GetSection("categories").Get<Category[]>())
            {
                await _categoriesRepository.AddAsync(category);
            }

            foreach (var question in configuration.GetSection("questions").Get<Question[]>())
            {
                await _questionRepository.AddAsync(question);

                var stub = new Answer
                {
                    QuestionId = question.Id,
                    IsStub = true,
                    Text = string.Empty
                };

                await _answerRepository.AddAsync(stub);
            }

            var users = await _userRepository.FindAsync(x => true);

            foreach (var user in users)
            {
                user.UserStatus = UserStatus.NewUser;
                user.IsWinner = false;

                await _userRepository.UpdateAsync(user);
            }

            _logger.LogDebug("DB updated");
        }
    }
}
