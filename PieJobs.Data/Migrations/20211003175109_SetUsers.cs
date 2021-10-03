using Microsoft.EntityFrameworkCore.Migrations;

namespace PieJobs.Data.Migrations
{
    public partial class SetUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ApiToken", "DisplayName", "Password", "UserName" },
                values: new object[] { 1, "eaf027ae-be97-4057-b872-efefe4ea4d22", "Admin", "5Mt/I3P5RbZrSXyI/k5FVz+lTL+ffWO+|10000|NbHfoOfFCJLUZCrSGZ/+VvMvNFB258cp", "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Name");
        }
    }
}
