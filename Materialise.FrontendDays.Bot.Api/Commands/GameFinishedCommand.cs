using System.Linq;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers;
using Materialise.FrontendDays.Bot.Api.Models;
using Materialise.FrontendDays.Bot.Api.Repositories.Contracts;
using Materialise.FrontendDays.Bot.Api.Resources;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class GameFinishedCommand : ICommand
    {
        private readonly Localization _localization;
        private readonly IDbRepository<Models.User> _useRepository;
        private readonly MessageSender _messageSender;
        private readonly ICategoryRepository _categoryRepository;

        public GameFinishedCommand(Localization localization,
            IDbRepository<Models.User> useRepository, MessageSender messageSender, 
            ICategoryRepository categoryRepository)
        {
            _localization = localization;
            _useRepository = useRepository;
            _messageSender = messageSender;
            _categoryRepository = categoryRepository;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;

            var user = (await _useRepository.FindAsync(x => x.Id == userId))
                .First();
            var category = await _categoryRepository.GetUserCategory(user.Id);

            await _messageSender.SendTo(userId, string.Format(_localization["allCorrectResponse"], 
                category.Description));

            user.UserStatus = UserStatus.Answered;

            await _useRepository.UpdateAsync(user);
        }
    }
}
