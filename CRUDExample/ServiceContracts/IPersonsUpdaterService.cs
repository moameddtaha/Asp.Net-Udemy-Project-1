using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represenets business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsUpdaterService
    {
        /// <summary>
        /// Updates the specified person details based on update, including person id
        /// </summary>
        /// <param name="personUpdateRequest"></param>
        /// <returns>Returns the person response object after updation</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }
}
