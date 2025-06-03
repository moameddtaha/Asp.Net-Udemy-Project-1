using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Infrastructure.Entities;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.ResultFilters;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepoCon;
using ContactManager.Infrastructure.Repositories;
using ServiceContracts;
using Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CRUDExample
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddTransient<ResponseHeaderActionFilter>();

            // It adds controllers and views as services
            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<ResponseHeaderActionFilter>(5);

                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                options.Filters.Add(new ResponseHeaderActionFilter(logger)
                {
                    Key = "my-key-from-global",
                    Value = "my-value-from-global",
                    Order = 2
                });
            });

            // Add services into IoC container
            services.AddScoped<ICountriesGetterService, CountriesGetterService>();
            services.AddScoped<ICountriesAdderService, CountriesAdderService>();
            services.AddScoped<ICountriesUploaderService, CountriesUploaderService>();

            services.AddScoped<PersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsGetterService, PersonGetterServiceWIthFewExcelFields>();

            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();

            services.AddScoped<ICountriesRepository, CountriesRepo>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddTransient<PersonsListResultFilter>();

            services.AddTransient<PersonsListActionFilter>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Enable Identity in this project
            services.AddIdentity<ApplicationUser, ApplicationRole>((options) =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 3; //Eg: ab12ab
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()

                .AddDefaultTokenProviders()

                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()

                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                    .Build(); // Enforeces authorization policy (user must be authenticated) for all action methods.

                options.AddPolicy("NotAuthorized", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return !context.User.Identity.IsAuthenticated;
                    });
                });
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            // Register HTTP Logging services with configuration
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
                                    | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            return services;
        }
    }
}
