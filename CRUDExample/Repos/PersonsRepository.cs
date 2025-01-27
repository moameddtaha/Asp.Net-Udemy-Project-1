using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepoCon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PersonsRepository> _logger;

        public PersonsRepository(ApplicationDbContext db, ILogger<PersonsRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonByPersonID(Guid? personID)
        {
            var person = await _db.Persons.SingleOrDefaultAsync(temp => temp.PersonID == personID);

            if (person != null)
            {
                _db.Persons.Remove(person);
                int rowsDeleted = await _db.SaveChangesAsync();
                return rowsDeleted > 0;
            }
            return false;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsRepository");

            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsRepository");

            return await _db.Persons.Include("Country")
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Person?> GetPersonbyPersonID(Guid? personID)
        {
            return await _db.Persons.Include("Country")
                .SingleOrDefaultAsync(temp => temp.PersonID == personID);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person), "Person cannot be null.");
            }

            Person? machingPerson = _db.Persons.SingleOrDefault(p => p.PersonID == person.PersonID);

            if (machingPerson == null)
            {
                throw new KeyNotFoundException($"No person founc with ID {person.PersonID}.");
            }

            machingPerson.PersonName = person.PersonName;
            machingPerson.Email = person.Email;
            machingPerson.DateOfBirth = person.DateOfBirth;
            machingPerson.CountryID = person.CountryID;
            machingPerson.Address = person.Address;
            machingPerson.ReceiveNewsLetter = person.ReceiveNewsLetter;

            await _db.SaveChangesAsync();

            return machingPerson;
        }
    }
}
