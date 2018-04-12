using System;
using System.Linq;
using System.Text;
using Materialise.FrontendDays.Bot.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Materialise.FrontendDays.Bot.Api.Filters
{
    public class BasicAuthenticationFilterAttribute : ActionFilterAttribute
    {
        public static Admin[] Admins { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Basic ".Length).Trim();
                var credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialString.Split(':');
                if (Admins.Any(x => x.Username == credentials[0] && x.Password == credentials[1]))
                {
                    return;
                }
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
