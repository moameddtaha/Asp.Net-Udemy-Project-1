using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepoCon;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesGetterService : ICountriesGetterService
    {
        //private field
        private readonly ICountriesRepository _countriesRepository;

        public CountriesGetterService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            List<Country> countries = await _countriesRepository.GetAllCountries();

            return countries
                .Select(country => country.ToCountryResponse())
                .ToList() ?? new List<CountryResponse>();

            // TODO: Return and implement the stored procedure for getting all countries

            //var countries = await _db.sp_GetAllCountries();
            //return countries.Select(temp => temp.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? CountryID)
        {
            if (CountryID == null)
            {
                return null;
            }
            Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(CountryID.Value);

            return country_response_from_list?.ToCountryResponse() ?? null;
        }

    }
}
