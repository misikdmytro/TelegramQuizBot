using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers.Contracts;
using Materialise.FrontendDays.Bot.Api.Resources;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands

{
    public class StartCommand : ICommand
    {
        private readonly Localization _localization;
        private readonly IUserRegistrationService _registrationService;
        private readonly IMessageSender _messageSender;

        public StartCommand(Localization localization, 
            IUserRegistrationService registrationService, IMessageSender messageSender)
        {
            _localization = localization;
            _registrationService = registrationService;
            _messageSender = messageSender;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = await _registrationService.RegisterIfNotExists(update);

            await _messageSender.SendTo(user.Id, 
                string.Format(_localization["helloFormat"], user.FirstName, user.LastName));
        }
    }
}
