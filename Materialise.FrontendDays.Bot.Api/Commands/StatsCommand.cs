using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Commands.Contracts;
using Materialise.FrontendDays.Bot.Api.Helpers.Contracts;
using Materialise.FrontendDays.Bot.Api.Properties;
using Materialise.FrontendDays.Bot.Api.Repositories.Contracts;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Materialise.FrontendDays.Bot.Api.Commands
{
    public class StatsCommand : ICommand
    {
        private readonly ILogger<StatsCommand> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMessageSender _messageSender;

        public StatsCommand(ICategoryRepository categoryRepository, ILogger<StatsCommand> logger, IMessageSender messageSender)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _messageSender = messageSender;
        }

        public async Task ExecuteAsync(Update update)
        {
            var userId = update.Message.From.Id;

            _logger.LogDebug($"User {userId} requests statistic");

            var builder = new StringBuilder(Resources.StatsHeaderFormat);

            var userCategories = (await _categoryRepository.GetUsersCategories())
                .GroupBy(x => x.Value);

            foreach (var userCategory in userCategories)
            {
                builder.AppendFormat(Resources.CategoryInfoFormat, userCategory.Key.Name,
                    userCategory.Count());
            }

            await _messageSender.SendTo(userId, builder.ToString());
        }
    }
}
