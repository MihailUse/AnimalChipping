using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Attributes;

public class ValidateIdentifierAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var identifierArguments = filterContext.ActionArguments
            .Where(x =>
                x.Key.Contains("Id", StringComparison.CurrentCultureIgnoreCase) &&
                x.Value is long or int
            );

        foreach (var (_, value) in identifierArguments)
        {
            if (value != null && Convert.ToInt64(value) > 0)
                continue;

            filterContext.Result = new BadRequestResult();
            return;
        }
    }
}