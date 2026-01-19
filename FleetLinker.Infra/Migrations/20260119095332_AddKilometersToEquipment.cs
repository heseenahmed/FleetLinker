using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetLinker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddKilometersToEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Kilometers",
                table: "Equipments",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kilometers",
                table: "Equipments");
        }
    }
}
