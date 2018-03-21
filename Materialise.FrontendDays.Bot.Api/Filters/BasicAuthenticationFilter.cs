using System;
using System.Linq;
using System.Text;
using Materialise.FrontendDays.Bot.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Materialise.FrontendDays.Bot.Api.Filters
{
    public class BasicAuthenticationFilter : ActionFilterAttribute
    {
        public static Admin[] Admins { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Basic ".Length).Trim();
                var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialstring.Split(':');
                if (!Admins.Any(x => x.Username == credentials[0] && x.Password == credentials[1]))
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
