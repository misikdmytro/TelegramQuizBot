using System.Threading.Tasks;

namespace Materialise.FrontendDays.Bot.Api.Helpers.Contracts
{
    public interface IMessageSender
    {
        Task SendTo(int userId, string message);
        Task SendTo(int userId, string message, params string[] options);
    }
}