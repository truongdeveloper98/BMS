using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class addUserType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserTypeId",
                table: "BS_Users",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "BS_UserTypes",
                columns: table => new
                {
                    UserTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BS_UserTypes", x => x.UserTypeId);
                });

            migrationBuilder.InsertData(
                table: "BS_UserTypes",
                columns: new[] { "UserTypeId", "Name" },
                values: new object[] { 1, "Intern" });

            migrationBuilder.InsertData(
                table: "BS_UserTypes",
                columns: new[] { "UserTypeId", "Name" },
                values: new object[] { 2, "Probation" });

            migrationBuilder.InsertData(
                table: "BS_UserTypes",
                columns: new[] { "UserTypeId", "Name" },
                values: new object[] { 3, "Official" });

            migrationBuilder.CreateIndex(
                name: "IX_BS_Users_UserTypeId",
                table: "BS_Users",
                column: "UserTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BS_Users_BS_UserTypes_UserTypeId",
                table: "BS_Users",
                column: "UserTypeId",
                principalTable: "BS_UserTypes",
                principalColumn: "UserTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BS_Users_BS_UserTypes_UserTypeId",
                table: "BS_Users");

            migrationBuilder.DropTable(
                name: "BS_UserTypes");

            migrationBuilder.DropIndex(
                name: "IX_BS_Users_UserTypeId",
                table: "BS_Users");

            migrationBuilder.DropColumn(
                name: "UserTypeId",
                table: "BS_Users");
        }
    }
}
