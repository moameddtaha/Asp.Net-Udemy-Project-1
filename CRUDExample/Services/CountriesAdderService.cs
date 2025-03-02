using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepoCon;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesAdderService : ICountriesAdderService
    {
        //private field
        private readonly ICountriesRepository _countriesRepository;

        public CountriesAdderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? request)
        {

            //Validation: CountryAddRequest parameter can't be null
            if (request == null)
            {
                throw new ArgumentNullException(nameof(CountryAddRequest));
            }

            // Validation: countryName can't be null
            if (request.CountryName == null)
            {
                throw new ArgumentException(nameof(CountryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            if (await _countriesRepository.GetCountryByCountryName(request.CountryName) != null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            // Convert Object from CountryAddRequest to Country type
            Country country = request.ToCountry();

            //Generate CountryID
            country.CountryId = Guid.NewGuid();

            //Add country object into _countries
            await _countriesRepository.AddCountry(country);

            // TODO: Return and implement the stored procedure for adding countries

            //await _db.sp_InsertCountries(country);

            return country.ToCountryResponse();
        }
    }
}
