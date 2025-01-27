using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="request">Country object to be added</param>
        /// <returns>Returns a country object after adding it (include newly generated country id</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? request);

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


        /// <summary>
        /// Uploads countries from excel file into database.
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns>Returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
