using System.Threading.Tasks;
using Telegram.Bot.Types;
using User = Materialise.FrontendDays.Bot.Api.Models.User;

namespace Materialise.FrontendDays.Bot.Api.Services.Contracts
{
    public interface IUserRegistrationService
    {
        Task<User> RegisterIfNotExists(Update update);
    }
}