using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BenzenyMain.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UserBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBranch_AspNetUsers_UserId",
                table: "UserBranch");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBranch_Branches_BranchId",
                table: "UserBranch");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBranch",
                table: "UserBranch");

            migrationBuilder.RenameTable(
                name: "UserBranch",
                newName: "UserBranchs");

            migrationBuilder.RenameIndex(
                name: "IX_UserBranch_BranchId",
                table: "UserBranchs",
                newName: "IX_UserBranchs_BranchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBranchs",
                table: "UserBranchs",
                columns: new[] { "UserId", "BranchId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranchs_AspNetUsers_UserId",
                table: "UserBranchs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranchs_Branches_BranchId",
                table: "UserBranchs",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBranchs_AspNetUsers_UserId",
                table: "UserBranchs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBranchs_Branches_BranchId",
                table: "UserBranchs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBranchs",
                table: "UserBranchs");

            migrationBuilder.RenameTable(
                name: "UserBranchs",
                newName: "UserBranch");

            migrationBuilder.RenameIndex(
                name: "IX_UserBranchs_BranchId",
                table: "UserBranch",
                newName: "IX_UserBranch_BranchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBranch",
                table: "UserBranch",
                columns: new[] { "UserId", "BranchId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranch_AspNetUsers_UserId",
                table: "UserBranch",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranch_Branches_BranchId",
                table: "UserBranch",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
