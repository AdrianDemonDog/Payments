using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Payments.Apps.User.Helpers;
using Payments.Apps.User.Models;
using System.Security.Claims;

public class Authorize : TypeFilterAttribute
    {
        public Authorize(string? roles = null) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { roles ?? "" };
        }
    }

public class AuthorizeFilter : IAuthorizationFilter
{
    private readonly string[] _requiredRoles;

    public AuthorizeFilter(string roles)
    {
        _requiredRoles = roles.Split(',').Select(r => r.Trim()).ToArray();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        bool isAllowAnonymous = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;
        if (isAllowAnonymous)
            return;

        var user = context.HttpContext.User;

        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        if (_requiredRoles.Length == 0)
            return;

        var userRoles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (_requiredRoles.Any(requiredRole => userRoles.Contains(requiredRole)))
        {
            return; // Permite el acceso
        }

        context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}
