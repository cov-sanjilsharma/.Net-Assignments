using Microsoft.AspNetCore.Mvc.Filters;
using Assignment3.Filters;

namespace Assignment3.Filters
{
    public class ActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("Action Started: " + context.ActionDescriptor.DisplayName);
        }
    }
}
