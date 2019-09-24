using System.Threading.Tasks;
using Telegram.Bot.Types;
using User = QuizBot.Api.Models.User;

namespace QuizBot.Api.Services.Interfaces
{
    public interface IUserRegistrationService
    {
        Task<User> RegisterIfNotExists(Update update);
    }
}