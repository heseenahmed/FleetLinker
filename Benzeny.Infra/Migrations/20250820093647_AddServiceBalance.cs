using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BenzenyMain.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRunAtUtc",
                table: "DriverFundingAssignment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRunAtUtc",
                table: "DriverFundingAssignment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRunAtUtc",
                table: "DriverFundingAssignment");

            migrationBuilder.DropColumn(
                name: "NextRunAtUtc",
                table: "DriverFundingAssignment");
        }
    }
}
