using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BenzenyMain.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCarTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarBrands_CarBrandId1",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarModels_CarModelId1",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarTypes_CarTypeId1",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_PlateTypes_PlateTypeId1",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_CarBrandId1",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_CarModelId1",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_CarTypeId1",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_PlateTypeId1",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "CarBrandId1",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "CarModelId1",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "CarTypeId1",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "PlateTypeId1",
                table: "Cars");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarBrandId1",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarModelId1",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarTypeId1",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlateTypeId1",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarBrandId1",
                table: "Cars",
                column: "CarBrandId1");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarModelId1",
                table: "Cars",
                column: "CarModelId1");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarTypeId1",
                table: "Cars",
                column: "CarTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_PlateTypeId1",
                table: "Cars",
                column: "PlateTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_CarBrands_CarBrandId1",
                table: "Cars",
                column: "CarBrandId1",
                principalTable: "CarBrands",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_CarModels_CarModelId1",
                table: "Cars",
                column: "CarModelId1",
                principalTable: "CarModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_CarTypes_CarTypeId1",
                table: "Cars",
                column: "CarTypeId1",
                principalTable: "CarTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_PlateTypes_PlateTypeId1",
                table: "Cars",
                column: "PlateTypeId1",
                principalTable: "PlateTypes",
                principalColumn: "Id");
        }
    }
}
