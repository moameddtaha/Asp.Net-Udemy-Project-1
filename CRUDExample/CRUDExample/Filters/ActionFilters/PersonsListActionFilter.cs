using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Runtime.CompilerServices;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        // After
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));

            PersonsController personsController = (PersonsController)context.Controller;

            Dictionary<string, object?>? parameters = (Dictionary<string, object?>?)context.HttpContext.Items["arguments"];

            if(parameters != null)
            {
                if(parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentsearchBy"] = Convert.ToString(parameters["searchBy"]);
                }
            }

            if (parameters != null)
            {
                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["CurrentsearchString"] = Convert.ToString(parameters["searchString"]);
                }
            }

            if (parameters != null)
            {
                if (parameters.ContainsKey("SortBy"))
                {
                    personsController.ViewData["SortBy"] = Convert.ToString(parameters["SortBy"]);
                }
                else
                {
                    personsController.ViewData["SortBy"] = nameof(PersonResponse.PersonName);
                }
            }
            
            if (parameters != null)
            {
                if (parameters.ContainsKey("SortOrder"))
                {
                    personsController.ViewData["SortOrder"] = Convert.ToString(parameters["SortOrder"]);
                }
                else
                {
                    personsController.ViewData["SortOrder"] = nameof(SortOrderOptions.ASC);
                }
            }

            // Search
            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryName), "Country" },
                { nameof(PersonResponse.Address), "Address" },
            };
        }

        // Before
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Pass the data to OnActionExecuted method via Items
            context.HttpContext.Items["arguments"] = context.ActionArguments;

            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                // 
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searcByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryName),
                        nameof(PersonResponse.Address),
                    };

                    // Resetting the searchBy parameter value
                    if (searcByOptions.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation($"searchBy actual value {searchBy}");

                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);

                        searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                        _logger.LogInformation($"searchBy updated value {searchBy}");
                    }
                }
            }
        }
    }
}
