using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.GeneralValueObjects.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AcademicManagement.Application;

public static class Auth
{
    public static IServiceCollection AddAndConfigureAuthenication(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddScheme<BasicRoleAuthOptions, BasicRoleAuthHandler>(
                BasicRoleAuthOptions.SchemaName, options =>
                {
                    options.PasswordToRole = new Dictionary<string, UserRole>
                    {
                        { "president_password", UserRole.President },
                        { "professor_password", UserRole.Professor }
                    };
                });

        return services;
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("PresidentOnly", x => x.RequireRole(UserRole.President.Value));
        });
        return services;
    }

}

public class BasicRoleAuthOptions : AuthenticationSchemeOptions
{
    public const string SchemaName = "BasicRoleAuth";
    public Dictionary<string, UserRole> PasswordToRole { get; set; } = [];
}

public class BasicRoleAuthHandler : AuthenticationHandler<BasicRoleAuthOptions>
{
    private readonly IUserRepository _userRepository;
    public BasicRoleAuthHandler(
        IOptionsMonitor<BasicRoleAuthOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserRepository userRepository)
        : base(options, logger, encoder, clock)
    {
        _userRepository = userRepository;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var (username, password) = GetUsernameAndPasswordFromRequest(Request);
        var role = await DoesUserExist(username, password);

        if (role is null)
        {
            return AuthenticateResult.Fail("Invalid Username or Password");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role.Value.Value)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private (string Username, string Password) GetUsernameAndPasswordFromRequest(HttpRequest request)
    {
        var authHeader = request.Headers["Authorization"].ToString();
        if (AuthenticationHeaderValue.TryParse(authHeader, out var headerValue))
        {
            if (headerValue.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
            {
                var credentials = System.Text.Encoding.UTF8
                    .GetString(Convert.FromBase64String(headerValue.Parameter ?? string.Empty))
                    .Split(':', 2);
                if (credentials.Length == 2)
                {
                    return (credentials[0], credentials[1]);
                }
            }
        }
        return (string.Empty, string.Empty);
    }

    private async Task<UserRole?> DoesUserExist(string username, string password)
    {
        Options.PasswordToRole.TryGetValue(password, out var expectedRole);
        var users = await _userRepository.GetAllUsers();
        var user = users.FirstOrDefault(u => u.Name.Value == username && u.Role == expectedRole);

        if (user is null)
        {
            return null;
        }

        return user.Role;
    }
}
