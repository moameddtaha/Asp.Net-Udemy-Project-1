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

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<PersonsListActionFilter>();

            // Enable Identity in this project
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()

                .AddDefaultTokenProviders()

                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()

                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

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
