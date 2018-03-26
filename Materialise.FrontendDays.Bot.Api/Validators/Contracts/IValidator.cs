using System.Threading.Tasks;

namespace Materialise.FrontendDays.Bot.Api.Validators.Contracts
{
    public interface IValidator<in TModel>
    {
        Task<bool> IsValid(TModel model);
    }
}
