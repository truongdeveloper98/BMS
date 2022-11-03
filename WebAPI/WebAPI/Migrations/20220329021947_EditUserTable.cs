using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class EditUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BS_Users_BS_UserTypes_UserTypeId",
                table: "BS_Users");

            migrationBuilder.DropIndex(
                name: "IX_BS_Users_UserTypeId",
                table: "BS_Users");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "BS_Users");

            migrationBuilder.DropColumn(
                name: "UserTypeId",
                table: "BS_Users");

            migrationBuilder.CreateIndex(
                name: "IX_BS_UserInfos_TypeId",
                table: "BS_UserInfos",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BS_UserInfos_BS_UserTypes_TypeId",
                table: "BS_UserInfos",
                column: "TypeId",
                principalTable: "BS_UserTypes",
                principalColumn: "UserTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BS_UserInfos_BS_UserTypes_TypeId",
                table: "BS_UserInfos");

            migrationBuilder.DropIndex(
                name: "IX_BS_UserInfos_TypeId",
                table: "BS_UserInfos");

            migrationBuilder.AddColumn<int>(
                name: "Department",
                table: "BS_Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserTypeId",
                table: "BS_Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BS_Users_UserTypeId",
                table: "BS_Users",
                column: "UserTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BS_Users_BS_UserTypes_UserTypeId",
                table: "BS_Users",
                column: "UserTypeId",
                principalTable: "BS_UserTypes",
                principalColumn: "UserTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
