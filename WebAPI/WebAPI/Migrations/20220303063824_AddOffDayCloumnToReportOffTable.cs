using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class AddOffDayCloumnToReportOffTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending",
                table: "BS_Vacations");

            migrationBuilder.AddColumn<double>(
                name: "OffDay",
                table: "ReportOffs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffDay",
                table: "ReportOffs");

            migrationBuilder.AddColumn<double>(
                name: "Pending",
                table: "BS_Vacations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
