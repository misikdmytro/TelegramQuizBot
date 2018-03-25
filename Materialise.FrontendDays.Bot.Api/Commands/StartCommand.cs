using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Resources;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class StartCommand : ICommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly Localization _localization;
        private readonly IUserRegistrationService _registrationService;

        public StartCommand(TelegramBotClient botClient, Localization localization, 
            IUserRegistrationService registrationService)
        {
            _botClient = botClient;
            _localization = localization;
            _registrationService = registrationService;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = await _registrationService.RegisterIfNotExists(update);

            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, 
                string.Format(_localization["helloFormat"], user.FirstName, user.LastName));
        }
    }
}
