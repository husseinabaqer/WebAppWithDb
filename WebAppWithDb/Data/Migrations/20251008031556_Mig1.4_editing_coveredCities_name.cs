using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppWithDb.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mig14_editing_coveredCities_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoverdCities_Drivers_DriverId",
                table: "CoverdCities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoverdCities",
                table: "CoverdCities");

            migrationBuilder.RenameTable(
                name: "CoverdCities",
                newName: "CoveredCities");

            migrationBuilder.RenameIndex(
                name: "IX_CoverdCities_DriverId",
                table: "CoveredCities",
                newName: "IX_CoveredCities_DriverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoveredCities",
                table: "CoveredCities",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoveredCities_Drivers_DriverId",
                table: "CoveredCities",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoveredCities_Drivers_DriverId",
                table: "CoveredCities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoveredCities",
                table: "CoveredCities");

            migrationBuilder.RenameTable(
                name: "CoveredCities",
                newName: "CoverdCities");

            migrationBuilder.RenameIndex(
                name: "IX_CoveredCities_DriverId",
                table: "CoverdCities",
                newName: "IX_CoverdCities_DriverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoverdCities",
                table: "CoverdCities",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoverdCities_Drivers_DriverId",
                table: "CoverdCities",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
