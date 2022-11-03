using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class editpartnerusersalaries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Created_By",
                table: "UserSalaries",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Created",
                table: "UserSalaries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserSalaries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Last_Updated",
                table: "UserSalaries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Updated_By",
                table: "UserSalaries",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ManMonth",
                table: "Projects",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Created_By",
                table: "PartnerInfos",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Created",
                table: "PartnerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PartnerInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Last_Updated",
                table: "PartnerInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Updated_By",
                table: "PartnerInfos",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Created_By",
                table: "BS_UserTypes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Created",
                table: "BS_UserTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BS_UserTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Last_Updated",
                table: "BS_UserTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Updated_By",
                table: "BS_UserTypes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Created_By",
                table: "BS_UserInfos",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Created",
                table: "BS_UserInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BS_UserInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Last_Updated",
                table: "BS_UserInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Updated_By",
                table: "BS_UserInfos",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created_By",
                table: "UserSalaries");

            migrationBuilder.DropColumn(
                name: "Date_Created",
                table: "UserSalaries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserSalaries");

            migrationBuilder.DropColumn(
                name: "Last_Updated",
                table: "UserSalaries");

            migrationBuilder.DropColumn(
                name: "Updated_By",
                table: "UserSalaries");

            migrationBuilder.DropColumn(
                name: "ManMonth",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Created_By",
                table: "PartnerInfos");

            migrationBuilder.DropColumn(
                name: "Date_Created",
                table: "PartnerInfos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PartnerInfos");

            migrationBuilder.DropColumn(
                name: "Last_Updated",
                table: "PartnerInfos");

            migrationBuilder.DropColumn(
                name: "Updated_By",
                table: "PartnerInfos");

            migrationBuilder.DropColumn(
                name: "Created_By",
                table: "BS_UserTypes");

            migrationBuilder.DropColumn(
                name: "Date_Created",
                table: "BS_UserTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BS_UserTypes");

            migrationBuilder.DropColumn(
                name: "Last_Updated",
                table: "BS_UserTypes");

            migrationBuilder.DropColumn(
                name: "Updated_By",
                table: "BS_UserTypes");

            migrationBuilder.DropColumn(
                name: "Created_By",
                table: "BS_UserInfos");

            migrationBuilder.DropColumn(
                name: "Date_Created",
                table: "BS_UserInfos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BS_UserInfos");

            migrationBuilder.DropColumn(
                name: "Last_Updated",
                table: "BS_UserInfos");

            migrationBuilder.DropColumn(
                name: "Updated_By",
                table: "BS_UserInfos");
        }
    }
}
