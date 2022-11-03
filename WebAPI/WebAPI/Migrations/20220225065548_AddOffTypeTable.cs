using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class AddOffTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OffType",
                table: "ReportOffs",
                newName: "OffTypeId");

            migrationBuilder.CreateTable(
                name: "BS_OffTypes",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Last_Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_By = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Updated_By = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BS_OffTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BS_OffTypes",
                columns: new[] { "Id", "Created_By", "Date_Created", "IsDeleted", "Last_Updated", "Name", "Updated_By" },
                values: new object[] { (byte)0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nghỉ phép", null });

            migrationBuilder.InsertData(
                table: "BS_OffTypes",
                columns: new[] { "Id", "Created_By", "Date_Created", "IsDeleted", "Last_Updated", "Name", "Updated_By" },
                values: new object[] { (byte)1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nghỉ không lương", null });

            migrationBuilder.InsertData(
                table: "BS_OffTypes",
                columns: new[] { "Id", "Created_By", "Date_Created", "IsDeleted", "Last_Updated", "Name", "Updated_By" },
                values: new object[] { (byte)2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nghỉ đặc biệt", null });

            migrationBuilder.CreateIndex(
                name: "IX_ReportOffs_OffTypeId",
                table: "ReportOffs",
                column: "OffTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportOffs_BS_OffTypes_OffTypeId",
                table: "ReportOffs",
                column: "OffTypeId",
                principalTable: "BS_OffTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportOffs_BS_OffTypes_OffTypeId",
                table: "ReportOffs");

            migrationBuilder.DropTable(
                name: "BS_OffTypes");

            migrationBuilder.DropIndex(
                name: "IX_ReportOffs_OffTypeId",
                table: "ReportOffs");

            migrationBuilder.RenameColumn(
                name: "OffTypeId",
                table: "ReportOffs",
                newName: "OffType");
        }
    }
}
