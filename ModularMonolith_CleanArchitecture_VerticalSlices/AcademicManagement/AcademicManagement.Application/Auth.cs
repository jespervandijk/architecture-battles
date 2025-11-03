using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Users;
using Marten;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vogen;

namespace AcademicManagement.Application;

public static class Auth
{
    public static IServiceCollection AuthenticationAcademicManagement(this IServiceCollection services)
    {
        services.AddAuthentication()
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
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyAcademicManagement.PresidentOnly.Value, x => x.RequireRole(UserRole.President.Value));
            options.AddPolicy(PolicyAcademicManagement.AdminOnly.Value, x => x.RequireRole(UserRole.Admin.Value));
            options.AddPolicy(PolicyAcademicManagement.ProfessorOnly.Value, x => x.RequireRole(UserRole.Professor.Value));
        });
        return services;
    }

}

[ValueObject<string>]
public partial struct PolicyAcademicManagement
{
    private static Validation Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Validation.Invalid("Policy cannot be empty.");
        }

        var allowedPolicies = new[] { "PresidentOnly", "AdminOnly", "ProfessorOnly" };
        if (Array.IndexOf(allowedPolicies, value) < 0)
        {
            return Validation.Invalid($"Policy '{value}' is not recognized.");
        }

        return Validation.Ok;
    }
    public static PolicyAcademicManagement PresidentOnly => From("PresidentOnly");
    public static PolicyAcademicManagement AdminOnly => From("AdminOnly");
    public static PolicyAcademicManagement ProfessorOnly => From("ProfessorOnly");
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
            new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new Claim(ClaimTypes.Name, user.Name.Value),
            new Claim(ClaimTypes.Role, user.Role.Value)
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

    private async Task<User> DoesUserExist(string username, string password)
    {
        Options.PasswordToRole.TryGetValue(password, out var expectedRole);
        var user = await _userRepository.Query().Where(x => x.Name.Value == username && x.Role == expectedRole).FirstOrDefaultAsync();

        if (user is null)
        {
            return null;
        }

        return user;
    }
}
