using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Person domain model class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }

        [StringLength(40)] //nvarchar(40)
        public string? PersonName { get; set; }

        [StringLength(40)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        // Unique Indentifier
        public Guid? CountryID { get; set; } // Foreign Key

        [StringLength(200)]
        public string? Address { get; set; }

        //bit
        public bool ReceiveNewsLetter { get; set; }

        //[Column("TaxIdentificationNumber", TypeName ="varchar(8)")]
        public string? TIN { get; set; }

        [ForeignKey("CountryID")]
        public virtual Country? Country { get; set; } // Navigation Property Name

        public override string? ToString()
        {
            return $"Person ID: {PersonID}, Person Name: {PersonName}, Email: {Email}, Date of Birth: {DateOfBirth?.ToString("dd/MMM/yyyy")}, Gender: {Gender}, Country ID: {Gender}, Country ID: {CountryID}, Address: {Address}, Receive News Letter: {ReceiveNewsLetter}, TIN: {TIN}, Country: {Country}";
        }
    }
}
