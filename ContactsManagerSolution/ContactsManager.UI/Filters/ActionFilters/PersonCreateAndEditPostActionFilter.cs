using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesAdderService _countriesAdderService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesUploaderService _countriesUploaderService;

        private readonly ILogger<PersonCreateAndEditPostActionFilter> _logger;

        public PersonCreateAndEditPostActionFilter(ICountriesAdderService countriesAdderService, ICountriesGetterService countriesGetterService, ICountriesUploaderService countriesUploaderService, ILogger<PersonCreateAndEditPostActionFilter> logger)
        {
            _countriesAdderService = countriesAdderService;
            _countriesGetterService = countriesGetterService;
            _countriesUploaderService = countriesUploaderService;

            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    var countries = await _countriesGetterService.GetAllCountries();
                    personsController.ViewBag.Countries = countries.Select(temp => new SelectListItem()
                    {
                        Text = temp.CountryName,
                        Value = temp.CountryId.ToString()
                    });

                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments["personRequest"];

                    context.Result = personsController.View(personRequest); // short-circuits or skips the subsequent action filters & action method
                }
                else
                {
                    await next(); // Invokes the subsequent filter or action method
                }
            }
            else
            {
                await next();
            }

            // After logic

            _logger.LogInformation("In after logic of PersonCreateAndEditPostActionFilter");

        }
    }
}