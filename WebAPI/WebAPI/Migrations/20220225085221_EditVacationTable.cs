using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class EditVacationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BS_Vacation_BS_Users_UserId",
                table: "BS_Vacation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BS_Vacation",
                table: "BS_Vacation");

            migrationBuilder.RenameTable(
                name: "BS_Vacation",
                newName: "BS_Vacations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BS_Vacations",
                table: "BS_Vacations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BS_Vacations_BS_Users_UserId",
                table: "BS_Vacations",
                column: "UserId",
                principalTable: "BS_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BS_Vacations_BS_Users_UserId",
                table: "BS_Vacations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BS_Vacations",
                table: "BS_Vacations");

            migrationBuilder.RenameTable(
                name: "BS_Vacations",
                newName: "BS_Vacation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BS_Vacation",
                table: "BS_Vacation",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BS_Vacation_BS_Users_UserId",
                table: "BS_Vacation",
                column: "UserId",
                principalTable: "BS_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
