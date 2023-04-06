using Application.Entities;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Attributes;

public class CheckRoleAttribute : ActionFilterAttribute
{
    private readonly AccountRole _role;

    public CheckRoleAttribute(AccountRole role)
    {
        _role = role;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var currentAccount = filterContext.HttpContext.RequestServices.GetService<ICurrentAccount>();

        if (currentAccount?.Account == null || !_role.HasFlag(currentAccount.Account.Role))
            filterContext.Result = new ObjectResult("Permission denied") { StatusCode = StatusCodes.Status403Forbidden };
    }
}