using System.Threading.Tasks;

namespace QuizBot.Api.Helpers.Interfaces
{
    public interface IMessageSender
    {
        Task SendTo(int userId, string message);
        Task SendTo(int userId, string message, params string[] options);
    }
}