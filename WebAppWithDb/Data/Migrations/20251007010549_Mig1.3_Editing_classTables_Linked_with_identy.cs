using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppWithDb.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mig13_Editing_classTables_Linked_with_identy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Students",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DecidedAt",
                table: "Requests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GenderPolicy",
                table: "Drivers",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Drivers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DecidedAt",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "GenderPolicy",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Drivers");
        }
    }
}
