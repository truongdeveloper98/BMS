using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class AddUserInfoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<int>(type: "int", nullable: false),
                    Team = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPending = table.Column<bool>(type: "bit", nullable: false, defaultValue: 0),
                    PendingStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PendingEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThisYearLeaveDay = table.Column<double>(type: "float", nullable: false, defaultValue: 0),
                    LastYearLeaveDay = table.Column<double>(type: "float", nullable: false, defaultValue: 0),
                    OccupiedLeaveDay = table.Column<double>(type: "float", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserInfos_BS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "BS_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInfos");
        }
    }
}
