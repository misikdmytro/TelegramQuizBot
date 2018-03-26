using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using User = Materialise.FrontendDays.Bot.Api.Models.User;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class RequestEmailCommand : ICommand
    {
        private readonly IDbRepository<User> _usersRepository;
        private readonly ILogger<RequestEmailCommand> _logger;
        private readonly MessageSender _messageSender;

        public RequestEmailCommand(IDbRepository<User> usersRepository, 
            ILogger<RequestEmailCommand> logger, MessageSender messageSender)
        {
            _usersRepository = usersRepository;
            _logger = logger;
            _messageSender = messageSender;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;

            var user = (await _usersRepository.FindAsync(x => x.Id == userId))
                .First();

            _logger.LogDebug($"Request user's {userId} e-mail");
            await _messageSender.SendTo(userId, "Please provide your e-mail");

            user.UserStatus = UserStatus.WaitForEmail;

            await _usersRepository.UpdateAsync(user);
        }
    }
}
