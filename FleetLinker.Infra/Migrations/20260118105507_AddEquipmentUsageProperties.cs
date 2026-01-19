using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetLinker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentUsageProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FuelLiters",
                table: "Equipments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MechanicalId",
                table: "Equipments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UsageHours",
                table: "Equipments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_MechanicalId",
                table: "Equipments",
                column: "MechanicalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_AspNetUsers_MechanicalId",
                table: "Equipments",
                column: "MechanicalId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_AspNetUsers_MechanicalId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_MechanicalId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "FuelLiters",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "MechanicalId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "UsageHours",
                table: "Equipments");
        }
    }
}
