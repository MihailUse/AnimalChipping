using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Attributes;

public class UnauthorizedAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.Headers.Authorization))
            filterContext.Result = new ForbidResult();
    }
}