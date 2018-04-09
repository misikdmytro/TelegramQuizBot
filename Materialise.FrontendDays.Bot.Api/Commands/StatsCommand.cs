using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers.Contracts;
using Materialise.FrontendDays.Bot.Api.Repositories.Contracts;
using Materialise.FrontendDays.Bot.Api.Resources;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class StatsCommand : ICommand
    {
        private readonly ILogger<StatsCommand> _logger;
        private readonly Localization _localization;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMessageSender _messageSender;

        public StatsCommand(ICategoryRepository categoryRepository, Localization localization, 
            ILogger<StatsCommand> logger, IMessageSender messageSender)
        {
            _categoryRepository = categoryRepository;
            _localization = localization;
            _logger = logger;
            _messageSender = messageSender;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;

            _logger.LogDebug($"User {userId} requests statistic");

            var builder = new StringBuilder(_localization["statsHeaderFormat"]);

            var userCategories = (await _categoryRepository.GetUsersCategories())
                .GroupBy(x => x.Value);

            foreach (var userCategory in userCategories)
            {
                builder.AppendFormat(_localization["categoryInfoFormat"], userCategory.Key.Name,
                    userCategory.Count());
            }

            await _messageSender.SendTo(userId, builder.ToString());
        }
    }
}
