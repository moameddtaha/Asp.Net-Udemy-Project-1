using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepoCon;
using ContactManager.Infrastructure.Repositories;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.ResultFilters;
using CRUDExample;
using CRUDExample.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) // Read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); // Read out current app's services and make them availble to serilog
});

// Add services
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

app.UseSerilogRequestLogging();

app.UseHttpLogging();

if (builder.Environment.IsEnvironment("Test") == false)
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseStaticFiles();

// Adds the authentication middleware to the request pipeline.
// This enables the app to recognize the signed-in user based on the authentication cookie or token.
app.UseAuthentication(); // Reading Identity cookie from the browser.
app.UseRouting(); // Identitifying action method based on route.
app.MapControllers(); // Execute the filter pipline (action + filter)

app.Run();

public partial class Program { } // Make the auto-generated Program accessible programmatically

