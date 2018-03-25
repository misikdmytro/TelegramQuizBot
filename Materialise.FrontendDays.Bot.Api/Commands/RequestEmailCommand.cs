using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Materialise.FrontendDays.Bot.Api.Models.User;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class RequestEmailCommand : ICommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly IDbRepository<User> _usersRepository;
        private readonly ILogger<RequestEmailCommand> _logger;

        public RequestEmailCommand(TelegramBotClient botClient, IDbRepository<User> usersRepository, 
            ILogger<RequestEmailCommand> logger)
        {
            _botClient = botClient;
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = (await _usersRepository.FindAsync(x => x.Id == update.Message.From.Id))
                .First();

            _logger.LogDebug($"Request user's {user.Id} e-mail");
            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Please provide your e-mail");

            user.UserStatus = UserStatus.WaitForEmail;

            await _usersRepository.UpdateAsync(user);
        }
    }
}
