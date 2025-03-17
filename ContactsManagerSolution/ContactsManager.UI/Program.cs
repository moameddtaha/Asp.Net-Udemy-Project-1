using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepoCon;
using Repos;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.ResultFilters;
using CRUDExample;
using CRUDExample.Middleware;

var builder = WebApplication.CreateBuilder(args);

// It adds controllers and views as services

builder.Services.AddTransient<ResponseHeaderActionFilter>();

builder.Services.AddControllersWithViews(options =>
{
    //options.Filters.Add<ResponseHeaderActionFilter>(5);

    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

    options.Filters.Add(new ResponseHeaderActionFilter(logger) 
    { 
        Key = "my-key-from-global",
        Value = "my-value-from-global",
        Order = 2 
    });
});

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
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { } // Make the auto-generated Program accessible programmatically

