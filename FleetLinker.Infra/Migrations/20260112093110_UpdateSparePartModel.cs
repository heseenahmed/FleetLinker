using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetLinker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSparePartModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "EquipmentSpareParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceHidden",
                table: "EquipmentSpareParts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                table: "EquipmentSpareParts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "EquipmentSpareParts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "EquipmentSpareParts");

            migrationBuilder.DropColumn(
                name: "IsPriceHidden",
                table: "EquipmentSpareParts");

            migrationBuilder.DropColumn(
                name: "Manufacturer",
                table: "EquipmentSpareParts");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "EquipmentSpareParts");
        }
    }
}
