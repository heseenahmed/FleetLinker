using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetLinker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEquipmentPropertyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsForSale",
                table: "Equipments",
                newName: "ForSale");

            migrationBuilder.RenameColumn(
                name: "IsForRent",
                table: "Equipments",
                newName: "ForRent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ForSale",
                table: "Equipments",
                newName: "IsForSale");

            migrationBuilder.RenameColumn(
                name: "ForRent",
                table: "Equipments",
                newName: "IsForRent");
        }
    }
}
