using AcademicManagement.Application;
using FastEndpoints;
using FastEndpoints.Swagger;
using Scalar.AspNetCore;
using StartUp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAndConfigureAuthenication();
builder.Services.AddAuthorizationPolicies();

builder.Services
    .AddFastEndpoints(options =>
        {
            options.Assemblies = ProjectRegistry.ApplicationLayers;
        })
    .SwaggerDocument(options =>
    {
        options.DocumentSettings =
            settings =>
            {
                settings.SchemaSettings.TypeMappers.AddValueObjectTypeMappers();
                settings.MarkNonNullablePropsAsRequired();
                settings.AddSecurity(BasicRoleAuthOptions.SchemaName, new NSwag.OpenApiSecurityScheme
                {
                    Type = NSwag.OpenApiSecuritySchemeType.Basic,
                    Name = "Authorization",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Scheme = "basic",
                    Description = "Input your username and password to access this API"
                });
            };
    });
builder.Services.AddServicesAllModules(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

app.Run();
