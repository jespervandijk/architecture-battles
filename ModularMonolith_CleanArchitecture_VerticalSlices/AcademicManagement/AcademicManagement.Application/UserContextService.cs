using System.Security.Claims;
using AcademicManagement.Domain.Aggregates.Users;
using Microsoft.AspNetCore.Http;

namespace AcademicManagement.Application;

public interface IUserContextService
{
    User GetCurrentUser();
}

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public User GetCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User.Identity?.IsAuthenticated is null or false)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var usernameClaim = httpContext.User.Identity.Name;
        var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
        var idClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (usernameClaim is null || idClaim is null || roleClaim is null)
        {
            throw new UnauthorizedAccessException("Required user claims are missing.");
        }

        return User.FromClaims(idClaim.Value, usernameClaim, roleClaim.Value);
    }

}
