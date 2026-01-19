using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetLinker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceRequestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaintenanceDescription",
                table: "EquipmentRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceResponse",
                table: "EquipmentRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenanceDescription",
                table: "EquipmentRequests");

            migrationBuilder.DropColumn(
                name: "MaintenanceResponse",
                table: "EquipmentRequests");
        }
    }
}
