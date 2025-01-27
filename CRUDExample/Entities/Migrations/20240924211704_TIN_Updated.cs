using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class TIN_Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TIN",
                table: "Persons",
                newName: "TaxIdentificationNumber");

            // Update all previous records where the value is NULL
            string sqlUpdate = @"
                UPDATE [dbo].[Persons]
                SET TaxIdentificationNumber = 'ABC12345'
                WHERE TaxIdentificationNumber = 'A-123456-Z';
            ";
            migrationBuilder.Sql(sqlUpdate);

            // Alter teh column to set a new default value
            migrationBuilder.AlterColumn<string>(
                name: "TaxIdentificationNumber",
                table: "Persons",
                type: "varchar(8)",
                nullable: true,
                defaultValue: "ABC12345",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaxIdentificationNumber",
                table: "Persons",
                newName: "TIN");

            migrationBuilder.AlterColumn<string>(
                name: "TIN",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldNullable: true,
                oldDefaultValue: "ABC12345");

            // Update all previous records where the value is NULL
            string sqlUpdate = @"
                UPDATE [dbo].[Persons]
                SET TaxIdentificationNumber = 'A-123456-Z'
                WHERE TaxIdentificationNumber = 'ABC12345';
            ";
            migrationBuilder.Sql(sqlUpdate);
        }
    }
}
