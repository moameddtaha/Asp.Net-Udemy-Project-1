
using Entities;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methds of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? CountryName { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetter { get; set; }
        public double? Age { get; set; }

        /// <summary>
        /// Compares the current object data with the parameters object
        /// </summary>
        /// <param name="obj">The PersonResponse object to compare</param>
        /// <returns>True or false, indicating whether all person details are matched with the specified parameter object</returns>
        public override bool Equals(object? obj)
        {
            if(obj == null)
            {
                return false;
            }

            if(obj.GetType() != typeof(PersonResponse))
            {
                return false;
            }

            PersonResponse person_to_compare = (PersonResponse)obj;

            return this.PersonID == person_to_compare.PersonID && this.PersonName == person_to_compare.PersonName && this.Email == person_to_compare.Email && this.DateOfBirth == person_to_compare.DateOfBirth && this.Gender == person_to_compare.Gender && this.CountryID == person_to_compare.CountryID && this.Address == person_to_compare.Address && this.ReceiveNewsLetter == person_to_compare.ReceiveNewsLetter;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person ID: {PersonID}, Person Name: {PersonName}, Email: {Email}, DatePfBirth: {DateOfBirth?.ToString("dd-MM-yyyy")}, Gender: {Gender}, CountryID: {CountryID}, Address: {Address}, ReceiveNewsLetter: {ReceiveNewsLetter}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = this.PersonID,
                PersonName = this.PersonName,
                Email = this.Email,
                DateOfBirth = this.DateOfBirth,
                Gender = string.IsNullOrEmpty(this.Gender) ? null : (GenderOptions)Enum.Parse(typeof(GenderOptions), this.Gender, true),
                CountryID = this.CountryID,
                Address = this.Address,
                ReceiveNewsLetter = this.ReceiveNewsLetter
            };
        }
    }
    public static class PersonExtentions
    {
        /// <summary>
        /// An Extention method to convert an object of a person class into PersonResponse class
        /// </summary>
        /// <param name="person">The person object to convert</param>
        /// <returns>Returns the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            // person => PersonResponse

            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReceiveNewsLetter = person.ReceiveNewsLetter,
                Age = CalculateAge(person.DateOfBirth),
                CountryName = person.Country?.CountryName
            };
        }

        private static double? CalculateAge(DateTime? dateOfBirth)
        {
            if(dateOfBirth == null) return null;

            return Math.Round((DateTime.Now - dateOfBirth.Value).TotalDays / 365);
        }
    }
}
