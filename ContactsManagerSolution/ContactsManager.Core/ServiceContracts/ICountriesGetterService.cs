using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesGetterService
    {
        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>Returns all countries from the list as List of CountryResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns a country object based on given country id
        /// </summary>
        /// <param name="CountryID">CountryID (guid) to search)</param>
        /// <returns>Match country as CountryResponse object</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? CountryID);
    }
}
