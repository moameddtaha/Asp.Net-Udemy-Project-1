using ContactsManager.Infrastructure.Entities;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepoCon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Infrastructure.Repositories
{
    public class CountriesRepo : ICountriesRepository
    {
        private readonly ApplicationDbContext _db;

        public CountriesRepo(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Country> AddCountry(Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _db.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByCountryID(Guid? CountryID)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryId == CountryID);
        }

        public async Task<Country?> GetCountryByCountryName(string CountryName)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryName == CountryName);
        }
    }
}
