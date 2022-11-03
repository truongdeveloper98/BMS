using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class addpartnerusersalaries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_BS_Posotions_PositionId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_BS_Posotions_PositionId",
                table: "UserProjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BS_Posotions",
                table: "BS_Posotions");

            migrationBuilder.RenameTable(
                name: "BS_Posotions",
                newName: "BS_Positions");

            migrationBuilder.RenameColumn(
                name: "Revenua",
                table: "Projects",
                newName: "Revenue");

            migrationBuilder.AddColumn<string>(
                name: "BackLogLink",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BS_Positions",
                table: "BS_Positions",
                column: "PositionId");

            migrationBuilder.CreateTable(
                name: "PartnerInfos",
                columns: table => new
                {
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vote = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerInfos", x => x.PartnerId);
                });

            migrationBuilder.CreateTable(
                name: "UserSalaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HourlySalary = table.Column<double>(type: "float", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSalaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSalaries_BS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "BS_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CustomerId",
                table: "Projects",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PartnerId",
                table: "Projects",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSalaries_UserId",
                table: "UserSalaries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_PartnerInfos_CustomerId",
                table: "Projects",
                column: "CustomerId",
                principalTable: "PartnerInfos",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_PartnerInfos_PartnerId",
                table: "Projects",
                column: "PartnerId",
                principalTable: "PartnerInfos",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_BS_Positions_PositionId",
                table: "Reports",
                column: "PositionId",
                principalTable: "BS_Positions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_BS_Positions_PositionId",
                table: "UserProjects",
                column: "PositionId",
                principalTable: "BS_Positions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_PartnerInfos_CustomerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_PartnerInfos_PartnerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_BS_Positions_PositionId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_BS_Positions_PositionId",
                table: "UserProjects");

            migrationBuilder.DropTable(
                name: "PartnerInfos");

            migrationBuilder.DropTable(
                name: "UserSalaries");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CustomerId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_PartnerId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BS_Positions",
                table: "BS_Positions");

            migrationBuilder.DropColumn(
                name: "BackLogLink",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "BS_Positions",
                newName: "BS_Posotions");

            migrationBuilder.RenameColumn(
                name: "Revenue",
                table: "Projects",
                newName: "Revenua");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BS_Posotions",
                table: "BS_Posotions",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_BS_Posotions_PositionId",
                table: "Reports",
                column: "PositionId",
                principalTable: "BS_Posotions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_BS_Posotions_PositionId",
                table: "UserProjects",
                column: "PositionId",
                principalTable: "BS_Posotions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
