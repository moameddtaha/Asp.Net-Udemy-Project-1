using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetAllCountries_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllCountries = @"
            CREATE PROCEDURE [dbo].[GetAllCountries]
            AS BEGIN
                SELECT CountryId, CountryName FROM [dbo].[Countries]
            END
            ";

            migrationBuilder.Sql(sp_GetAllCountries);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllCountries = @"
            DROP PROCEDURE [dbo].[GetAllCountries]";

            migrationBuilder.Sql(sp_GetAllCountries);
        }
    }
}
