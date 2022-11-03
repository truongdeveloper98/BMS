using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class RemoveForeignKeyPositionId1ToTableReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_BS_Posotions_PositionId1",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_PositionId1",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "PositionId1",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "PositionId",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_PositionId",
                table: "Reports",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_BS_Posotions_PositionId",
                table: "Reports",
                column: "PositionId",
                principalTable: "BS_Posotions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_BS_Posotions_PositionId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_PositionId",
                table: "Reports");

            migrationBuilder.AlterColumn<string>(
                name: "PositionId",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PositionId1",
                table: "Reports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_PositionId1",
                table: "Reports",
                column: "PositionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_BS_Posotions_PositionId1",
                table: "Reports",
                column: "PositionId1",
                principalTable: "BS_Posotions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
