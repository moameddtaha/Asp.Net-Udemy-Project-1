using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertCountry_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertCountries = @"
            CREATE PROCEDURE [dbo].[InsertCountry]
            (
                @CountryName nvarchar(40)
            )
            AS
            BEGIN
                INSERT INTO [dbo].[Countries]
                (
                    CountryName
                )
                VALUES
                (
                    @CountryName
                )
            END
            ";

            migrationBuilder.Sql(sp_InsertCountries);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertCountries = @"DROP PROCEDURE [dbo][InsertCountry]";
            migrationBuilder.Sql(sp_InsertCountries);
        }
    }
}
