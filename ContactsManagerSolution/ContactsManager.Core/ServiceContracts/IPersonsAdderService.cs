using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represenets business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Adds a new person into the list of persons
        /// </summary>
        /// <param name="request">Person to be added</param>
        /// <returns>Returns the same person details, along with newly generated PersonID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? request);
    }
}
