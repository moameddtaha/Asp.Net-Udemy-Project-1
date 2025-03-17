using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepoCon
{
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a person object to the data store
        /// </summary>
        /// <param name="person"></param>
        /// <returns>Returns the person object after adding it to the table</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all persons in the data store
        /// </summary>
        /// <returns>List of person objects from table</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Returns a person object based on the given person id
        /// </summary>
        /// <param name="personID"></param>
        /// <returns>A person object or null</returns>
        Task<Person?> GetPersonbyPersonID(Guid? personID);

        /// <summary>
        /// Returns all persons objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Updates a person object (person name and other details) based on the given person id
        /// </summary>
        /// <param name="person">Person object to update</param>
        /// <returns>Returns the updated person</returns>
        Task<Person> UpdatePerson(Person person);

        /// <summary>
        /// Deletes a person object based on the person id
        /// </summary>
        /// <param name="personID">Person ID (guid) to search</param>
        /// <returns>Returns true, if the deletion is sucessful; otherwise false</returns>
        Task<bool> DeletePersonByPersonID(Guid? personID);
    }
}
