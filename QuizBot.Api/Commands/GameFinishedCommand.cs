using System.Threading.Tasks;
using QuizBot.Api.Commands.Interfaces;
using QuizBot.Api.Helpers.Interfaces;
using QuizBot.Api.Models;
using QuizBot.Api.Properties;
using QuizBot.Api.Repositories.Interfaces;
using QuizBot.Api.Services.Interfaces;
using Telegram.Bot.Types;

namespace QuizBot.Api.Commands
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
