using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Materialise.FrontendDays.Bot.Api.Validators.Contracts;

namespace Materialise.FrontendDays.Bot.Api.Validators
{
    public class EmailValidator : IValidator<string>
    {
        public Task<bool> IsValid(string model)
        {
            try
            {
                var mailAddress = new MailAddress(model);

                return Task.FromResult(true);
            }
            catch (FormatException)
            {
                return Task.FromResult(false);
            }
        }
    }
}
