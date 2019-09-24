using System.Threading.Tasks;
using QuizBot.Api.Commands.Interfaces;
using QuizBot.Api.Helpers.Interfaces;
using QuizBot.Api.Properties;
using QuizBot.Api.Services.Interfaces;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands
{
    public class StartCommand : ICommand
    {
        private readonly IUserRegistrationService _registrationService;
        private readonly IMessageSender _messageSender;

        public StartCommand(IUserRegistrationService registrationService, IMessageSender messageSender)
        {
            _registrationService = registrationService;
            _messageSender = messageSender;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = await _registrationService.RegisterIfNotExists(update);

            await _messageSender.SendTo(user.Id, 
                string.Format(Resources.HelloFormat, user.FirstName, user.LastName));
        }
    }
}
