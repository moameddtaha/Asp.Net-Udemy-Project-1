using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPersons = @"
            CREATE PROCEDURE [dbo].[UpdatePerson]
            (
                @PersonID uniqueidentifier,
                @PersonName nvarchar(40),
                @Email nvarchar(40),
                @DateOfBirth datetime2(7),
                @Gender nvarchar(10),
                @CountryID uniqueidentifier,
                @Address nvarchar(200),
                @ReceiveNewsLetter bit
            )
            AS BEGIN
                -- Check if PersonID exists
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Persons] WHERE PersonID = @PersonID)
                BEGIN
                    RAISERROR('Given person id does not exist.', 16, 1);
                    RETURN;
                END
                
                -- Upate the record

                UPDATE [dbo].[Persons]
                SET PersonName = @PersonName,
                    Email = @Email,
                    DateOfBirth = @DateOfBirth,
                    Gender = @Gender,
                    CountryID = @CountryID,
                    Address = @Address,
                    ReceiveNewsLetter = @ReceiveNewsLetter
                WHERE PersonID = @PersonID;
            END
            ";

            migrationBuilder.Sql(sp_InsertPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_UpdatePerson = @"DROP PROCEDURE [db].[UpdatePerson]";
            migrationBuilder.Sql(sp_UpdatePerson);
        }
    }
}
