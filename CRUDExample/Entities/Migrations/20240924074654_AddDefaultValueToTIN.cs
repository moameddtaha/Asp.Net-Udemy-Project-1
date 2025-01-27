using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultValueToTIN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string addDefaultConstraint = @"
                ALTER TABLE [dbo].[Persons]
                ADD CONSTRAINT DF_Persons_TIN
                DEFAULT 'A-123456-Z'
                FOR TIN;
            ";

            migrationBuilder.Sql(addDefaultConstraint);

            // Update existing records to set TIN to default value if it's null
            string updateExistingRecords = @"
                UPDATE [dbo].[Persons]
                SET TIN = 'A-123456-Z'
                WHERE TIN IS NULL;
            ";
            migrationBuilder.Sql(updateExistingRecords);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string dropDefaultConstraint = @"
                ALTER TABLE [dbo].[Persons]
                DROP CONSTRAINT DF_Persons_TIN;
            ";
            migrationBuilder.Sql(dropDefaultConstraint);
        }
    }
}
