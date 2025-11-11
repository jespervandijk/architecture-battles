using AcademicManagement.Application;
using FastEndpoints;
using FastEndpoints.Swagger;
using StartUp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AuthenticationAcademicManagement();
builder.Services.AuthorizationAcademicManagement();

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
                _ = settings.AddSecurity(BasicRoleAuthOptions.SchemaName, new NSwag.OpenApiSecurityScheme
                {
                    Type = NSwag.OpenApiSecuritySchemeType.Basic,
                    Name = "Authorization",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Scheme = "basic",
                    Description = "Input your username and password to access this API"
                });
            };
        options.ShortSchemaNames = true;
        options.AutoTagPathSegmentIndex = 0;
    });

builder.Services.AddServicesAllModules(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints()
    .UseSwaggerGen();

app.Run();
