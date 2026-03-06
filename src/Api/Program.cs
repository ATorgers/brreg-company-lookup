using Carter;
using Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddMediator();

var brregBaseUrl = builder.Configuration["Brreg:BaseUrl"]
    ?? throw new InvalidOperationException("'Brreg:BaseUrl' is not configured in appsettings.");

builder.Services.AddInfrastructure(brregBaseUrl);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();

app.MapCarter();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
