using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Models;

namespace Materialise.FrontendDays.Bot.Api.Services.Contracts
{
    public interface IUserService
    {
        Task<User[]> GetPossibleWinnersAsync();
    }
}
