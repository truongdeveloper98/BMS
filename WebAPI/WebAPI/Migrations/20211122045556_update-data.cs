using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class updatedata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectTypes_TypeId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_BS_Roles_RoleId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_UserProjects_ProjectId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_BS_Roles_RoleId",
                table: "UserProjects");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UserProjects_ProjectId",
                table: "UserProjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProjects",
                table: "UserProjects");

            migrationBuilder.DropIndex(
                name: "IX_UserProjects_RoleId",
                table: "UserProjects");

            migrationBuilder.DropIndex(
                name: "IX_Reports_RoleId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Projects_TypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UserProjects");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProjectTypes",
                newName: "ProjectType_Name");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Projects",
                newName: "Tester_Estimate");

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "UserProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PositionId",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositionId1",
                table: "Reports",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Projects",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<int>(
                name: "Brse_Estimate",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Comtor_Estimate",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Developer_Estimate",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PM_Estimate",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Revenua",
                table: "Projects",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProjects",
                table: "UserProjects",
                columns: new[] { "UserId", "ProjectId", "PositionId" });

            migrationBuilder.CreateTable(
                name: "BS_Posotions",
                columns: table => new
                {
                    PositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Last_Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_By = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Updated_By = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BS_Posotions", x => x.PositionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProjects_PositionId",
                table: "UserProjects",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjects_ProjectId",
                table: "UserProjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_PositionId1",
                table: "Reports",
                column: "PositionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectTypeId",
                table: "Projects",
                column: "ProjectTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectTypes_ProjectTypeId",
                table: "Projects",
                column: "ProjectTypeId",
                principalTable: "ProjectTypes",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_BS_Posotions_PositionId1",
                table: "Reports",
                column: "PositionId1",
                principalTable: "BS_Posotions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Projects_ProjectId",
                table: "Reports",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_BS_Posotions_PositionId",
                table: "UserProjects",
                column: "PositionId",
                principalTable: "BS_Posotions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectTypes_ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_BS_Posotions_PositionId1",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Projects_ProjectId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_BS_Posotions_PositionId",
                table: "UserProjects");

            migrationBuilder.DropTable(
                name: "BS_Posotions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProjects",
                table: "UserProjects");

            migrationBuilder.DropIndex(
                name: "IX_UserProjects_PositionId",
                table: "UserProjects");

            migrationBuilder.DropIndex(
                name: "IX_UserProjects_ProjectId",
                table: "UserProjects");

            migrationBuilder.DropIndex(
                name: "IX_Reports_PositionId1",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "UserProjects");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "PositionId1",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Brse_Estimate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Comtor_Estimate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Developer_Estimate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PM_Estimate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Revenua",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "ProjectType_Name",
                table: "ProjectTypes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Tester_Estimate",
                table: "Projects",
                newName: "TypeId");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "UserProjects",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "Reports",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Projects",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UserProjects_ProjectId",
                table: "UserProjects",
                column: "ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProjects",
                table: "UserProjects",
                columns: new[] { "UserId", "ProjectId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProjects_RoleId",
                table: "UserProjects",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_RoleId",
                table: "Reports",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_TypeId",
                table: "Projects",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectTypes_TypeId",
                table: "Projects",
                column: "TypeId",
                principalTable: "ProjectTypes",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_BS_Roles_RoleId",
                table: "Reports",
                column: "RoleId",
                principalTable: "BS_Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_UserProjects_ProjectId",
                table: "Reports",
                column: "ProjectId",
                principalTable: "UserProjects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_BS_Roles_RoleId",
                table: "UserProjects",
                column: "RoleId",
                principalTable: "BS_Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
