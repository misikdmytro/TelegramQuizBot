using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Extensions;
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

        public UpdateDatabaseHandler(IDbRepository<Answer> answersRepository, 
            ILogger<UpdateDatabaseHandler> logger, IDbRepository<Question> questionRepository, 
            IUserAnswerRepository userAnswerRepository, IDbRepository<User> userRepository)
        {
            _answersRepository = answersRepository;
            _logger = logger;
            _questionRepository = questionRepository;
            _userAnswerRepository = userAnswerRepository;
            _userRepository = userRepository;
        }

        public async Task Handle(UpdateDatabaseRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Updating DB...");

            await _answersRepository.ClearAsync();
            await _questionRepository.ClearAsync();
            await _userAnswerRepository.ClearAsync();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            foreach (var question in configuration.GetSection("questions").Get<Question[]>())
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
        }
    }
}
