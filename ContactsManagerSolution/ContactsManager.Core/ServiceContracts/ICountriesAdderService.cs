using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesAdderService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="request">Country object to be added</param>
        /// <returns>Returns a country object after adding it (include newly generated country id</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? request);
    }
}
