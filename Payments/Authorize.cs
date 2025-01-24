using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Payments.Apps.User.Helpers;
using Payments.Apps.User.Models;

public class Authorize : TypeFilterAttribute
    {
        public Authorize(string? roles = null) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { roles ?? "" };
        }
    }

public class AuthorizeFilter : IAuthorizationFilter
{
    public readonly string _requiredRoles;

    public AuthorizeFilter(string roles)
    {
        _requiredRoles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // CHECK ALLOW ANONYMOUS
        bool isAllowAnonymous = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;
        if (isAllowAnonymous == true)
            return;

        var _user = context.HttpContext.User;
        if (_user.Identity != null && _user.Identity.IsAuthenticated)
        {
            // IF ONLY IS [AUTHORIZE]
            if (_requiredRoles == "")
                return;

            var userId = _user.Claims.FirstOrDefault(c => c.Type == "uId")?.Value;
            if (userId != null)
            {
                UserModel user = UserHelper.GetUserById(userId);
                if (user != null && user.Roles != null)
                {
                    if (user.Roles.Contains(_requiredRoles))
                    {
                        return;
                    }
                }
            }
        }
        context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}
