using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class DeletePerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_DeletetPersons = @"
            CREATE PROCEDURE [dbo].[DeletePerson]
            (
                @PersonID uniqueidentifier
            )
            AS BEGIN
                -- Check if PersonID exists
                IF NOT EXISTS (SELECT 1 FROM [dbo].[Persons] WHERE PersonID = @PersonID)
                BEGIN
                    RAISERROR('Given person id does not exist.', 16, 1);
                    RETURN;
                END
                
                -- Delete the record

                DELETE [dbo].[Persons]
                WHERE PersonID = @PersonID;
            END
            ";

            migrationBuilder.Sql(sp_DeletetPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_DeletetPersons = @"DROP PROCEDURE [db].[DeletePerson]";
            migrationBuilder.Sql(sp_DeletetPersons);
        }
    }
}
