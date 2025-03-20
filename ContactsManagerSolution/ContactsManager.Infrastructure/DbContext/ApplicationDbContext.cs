using ContactsManager.Core.Domain.IdentityEntities;
using Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // Seed to Countries
            string countriesJson = File.ReadAllText("Resources/countries.json");
            List<Country>? countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            if (countries != null)
            {
                foreach (Country country in countries)
                {
                    modelBuilder.Entity<Country>().HasData(country);
                }
            }

            // Seed to Persons
            string personsJson = File.ReadAllText("Resources/persons.json");
            List<Person>? perosns = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            if (perosns != null)
            {
                foreach (Person perosn in perosns)
                {
                    modelBuilder.Entity<Person>().HasData(perosn);
                }
            }


            // Fluent API
            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

            //modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN)
            //    .IsUnique();

            modelBuilder.Entity<Person>()
                .ToTable("Persons", t=> t.HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8"));

            //// Table Relations
            //modelBuilder.Entity<Person>()
            //    .HasOne(c => c.Country)
            //    .WithMany(p => p.Persons)
            //    .HasForeignKey(p => p.CountryID);
        }

        #region Person Stored Procedure Methods

        public async Task<IList<Person>> sp_GetAllPersons()
        {
            return await Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToListAsync();
        }
        public async Task<int> sp_InsertPersons(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetter", person.ReceiveNewsLetter),
            };
            return await Database.ExecuteSqlRawAsync("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetter", parameters);
        }
        public async Task<int> sp_UpdatePerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetter", person.ReceiveNewsLetter),
            };
            return await Database.ExecuteSqlRawAsync("EXECUTE [dbo].[UpdatePerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetter", parameters);
        }

        public async Task<int> sp_DeletePerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@PersonID", person.PersonID),
            };
            return await Database.ExecuteSqlRawAsync("EXECUTE [dbo].[DeletePerson] @PersonID", parameters);
        }

        #endregion

        #region Country Stored Procedure Methods

        public async Task<IList<Country>> sp_GetAllCountries()
        {
            return await Countries.FromSqlRaw("EXECUTE [dbo].[GetAllCountries]").ToListAsync();
        }
        public async Task<int> sp_InsertCountries(Country country)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@CountryId", country.CountryId),
                new SqlParameter("@CountryName", country.CountryName)
            };
            return await Database.ExecuteSqlRawAsync("EXECUTE [dbo].[InsertCountry] @CountryId, @CountryName", parameters);
        }

        #endregion


    }
}
