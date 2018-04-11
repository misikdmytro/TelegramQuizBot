using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers.Contracts;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Properties;
using Materialise.FrontendDays.Bot.Api.Repositories.Contracts;
using Materialise.FrontendDays.Bot.Api.Services.Contracts;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class GameFinishedCommand : ICommand
    {
        private readonly IDbRepository<Models.User> _useRepository;
        private readonly IMessageSender _messageSender;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRegistrationService _userRegistrationService;

        public GameFinishedCommand(IDbRepository<Models.User> useRepository, IMessageSender messageSender, 
            ICategoryRepository categoryRepository, IUserRegistrationService userRegistrationService)
        {
            _useRepository = useRepository;
            _messageSender = messageSender;
            _categoryRepository = categoryRepository;
            _userRegistrationService = userRegistrationService;
        }

        public async Task ExecuteAsync(Update update)
        {
            var user = await _userRegistrationService.RegisterIfNotExists(update);

            user.UserStatus = UserStatus.Answered;
            await _useRepository.UpdateAsync(user);

            var category = await _categoryRepository.GetUserCategory(user.Id);

            await _messageSender.SendTo(user.Id, string.Format(Resources.AllCorrectResponse, 
                category.Name, category.Description));
        }
    }
}
