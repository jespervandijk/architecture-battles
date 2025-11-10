using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AcademicManagement.Application;

public static class Auth
{
    public static IServiceCollection AuthenticationAcademicManagement(this IServiceCollection services)
    {
        _ = services.AddAuthentication()
           .AddScheme<BasicRoleAuthOptions, BasicRoleAuthHandler>(
               BasicRoleAuthOptions.SchemaName, options =>
               {
                   options.PasswordToRole = new Dictionary<string, UserRole>
                   {
                        // todo: hard coded passwords to appsettings at least
                        { "president_password", UserRole.President },
                        { "professor_password", UserRole.Professor }
                   };
               });
        return services;
    }

    public static IServiceCollection AuthorizationAcademicManagement(this IServiceCollection services)
    {
        return services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyAcademicManagement.PresidentOnly, x => x.RequireRole(UserRole.President.ToString()));
            options.AddPolicy(PolicyAcademicManagement.ProfessorOnly, x => x.RequireRole(UserRole.Professor.ToString()));
        });
    }

}

public static class PolicyAcademicManagement
{
    public const string PresidentOnly = "PresidentOnly";
    public const string ProfessorOnly = "ProfessorOnly";
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

        // todo: Hard coded admin check (with appsettings at least)

        var user = await DoesUserExist(username, password);

        if (user is null)
        {
            return AuthenticateResult.Fail("Invalid Username or Password");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new(ClaimTypes.Name, user.Name.Value),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private static (string Username, string Password) GetUsernameAndPasswordFromRequest(HttpRequest request)
    {
        var authHeader = request.Headers.Authorization.ToString();
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

    private async Task<User?> DoesUserExist(string username, string password)
    {
        _ = Options.PasswordToRole.TryGetValue(password, out var expectedRole);
        var allUsers = await _userRepository.GetAllAsync();
        return allUsers
            .FirstOrDefault(u => u.Name.Value.Equals(username, StringComparison.OrdinalIgnoreCase)
                && u.Role == expectedRole);
    }
}
