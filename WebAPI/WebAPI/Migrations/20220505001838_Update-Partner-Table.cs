using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class UpdatePartnerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustomer",
                table: "PartnerInfos",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPartner",
                table: "PartnerInfos",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "BS_UserInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCustomer",
                table: "PartnerInfos");

            migrationBuilder.DropColumn(
                name: "IsPartner",
                table: "PartnerInfos");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "BS_UserInfos");
        }
    }
}
