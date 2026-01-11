using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetLinker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class EquipmentSparePartsLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "EquipmentSpareParts",
                newName: "BrandEn");

            migrationBuilder.AddColumn<string>(
                name: "BrandAr",
                table: "EquipmentSpareParts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandAr",
                table: "EquipmentSpareParts");

            migrationBuilder.RenameColumn(
                name: "BrandEn",
                table: "EquipmentSpareParts",
                newName: "Brand");
        }
    }
}
