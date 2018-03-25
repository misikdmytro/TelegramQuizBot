using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using User = Materialise.FrontendDays.Bot.Api.Models.User;

namespace Materialise.FrontendDays.Bot.Api.Services
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
                Email = string.Empty,
                IsWinner = false
            };

            _logger.LogDebug($"New user registered: {newUser.FirstName} {newUser.LastName}");
            await _usersRepository.AddAsync(newUser);

            return newUser;
        }
    }
}
