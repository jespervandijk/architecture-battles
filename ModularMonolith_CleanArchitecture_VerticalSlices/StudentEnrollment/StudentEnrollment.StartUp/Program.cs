using FastEndpoints;
using FastEndpoints.Swagger;
using Scalar.AspNetCore;
using StudentEnrollment.Application;
using StudentEnrollment.StartUp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services
    .AddFastEndpoints(options =>
        {
            options.Assemblies = [typeof(DependencyInjectionApplication).Assembly];
        })
    .SwaggerDocument(options =>
    {
        options.DocumentSettings =
            settings => settings.SchemaSettings.TypeMappers.AddValueObjectTypeMappers();
    });

var app = builder.Build();

app.UseFastEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(c => c.Path = "/openapi/{documentName}.json");
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.Run();