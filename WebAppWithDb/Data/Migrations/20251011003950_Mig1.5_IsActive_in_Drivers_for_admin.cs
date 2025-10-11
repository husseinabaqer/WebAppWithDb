using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppWithDb.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mig15_IsActive_in_Drivers_for_admin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Drivers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Drivers");
        }
    }
}
