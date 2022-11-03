using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class UpdateUserInfoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_BS_Users_UserId",
                table: "UserInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserInfos",
                table: "UserInfos");

            migrationBuilder.RenameTable(
                name: "UserInfos",
                newName: "BS_UserInfos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BS_UserInfos",
                table: "BS_UserInfos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BS_UserInfos_BS_Users_UserId",
                table: "BS_UserInfos",
                column: "UserId",
                principalTable: "BS_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BS_UserInfos_BS_Users_UserId",
                table: "BS_UserInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BS_UserInfos",
                table: "BS_UserInfos");

            migrationBuilder.RenameTable(
                name: "BS_UserInfos",
                newName: "UserInfos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserInfos",
                table: "UserInfos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfos_BS_Users_UserId",
                table: "UserInfos",
                column: "UserId",
                principalTable: "BS_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
