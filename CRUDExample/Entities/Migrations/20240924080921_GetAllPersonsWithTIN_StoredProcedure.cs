using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetAllPersonsWithTIN_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
            ALTER PROCEDURE [dbo].[GetAllPersons]
            AS BEGIN
                SELECT PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetter, TIN
                FROM [dbo].[Persons]
            END
            ";

            migrationBuilder.Sql(sp_GetAllPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
            CREATE PROCEDURE [dbo].[GetAllPersons]
            AS BEGIN
                SELECT PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetter FROM [dbo].[Persons]
            END
            ";

            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
