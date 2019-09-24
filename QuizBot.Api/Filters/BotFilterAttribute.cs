using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuizBot.Api.Filters
{
    public class BotFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null && !context.ExceptionHandled)
            {
                context.Result = new OkResult();
                context.ExceptionHandled = true;
            }

            base.OnActionExecuted(context);
        }
    }
}
