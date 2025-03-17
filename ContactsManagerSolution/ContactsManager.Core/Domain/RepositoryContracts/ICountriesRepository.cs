using Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepoCon
{
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns the country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all countries in the data store
        /// </summary>
        /// <returns>All Countries from the table</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns a country object based on the given country id; otherwise, it returns null
        /// </summary>
        /// <param name="CountryID"></param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryID(Guid? CountryID);

        /// <summary>
        /// Returns a country object based on the given country name
        /// </summary>
        /// <param name="CountryName"></param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string CountryName);
    }
}
