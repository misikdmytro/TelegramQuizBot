using System.Linq;
using System.Threading.Tasks;
using QuizBot.Api.Models;
using Microsoft.Extensions.Logging;
using QuizBot.Api.Repositories.Interfaces;
using QuizBot.Api.Services.Interfaces;
using Telegram.Bot.Types;
using User = QuizBot.Api.Models.User;

namespace QuizBot.Api.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IDbRepository<User> _usersRepository;
        private readonly ILogger<UserRegistrationService> _logger;

        public UserRegistrationService(IDbRepository<User> usersRepository, 
            ILogger<UserRegistrationService> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public async Task<User> RegisterIfNotExists(Update update)
        {
            var user = (await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id))
                .FirstOrDefault();

            if (user != null)
            {
                return user;
            }

            var newUser = new User
            {
                LastName = update.Message.From.LastName,
                Id = update.Message.From.Id,
                FirstName = update.Message.From.FirstName,
                Username = update.Message.From.Username,
                ChatId = update.Message.Chat.Id,
                UserStatus = UserStatus.NewUser,
                IsWinner = false
            };

            _logger.LogDebug($"New user registered: {newUser.FirstName} {newUser.LastName}");
            await _usersRepository.AddAsync(newUser);

            return newUser;
        }
    }
}
