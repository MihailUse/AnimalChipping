using Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class AddAccountRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Accounts",
                type: "integer",
                nullable: false,
                defaultValue: AccountRole.USER);

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[,]
                {
                    { 1, "admin@simbirsoft.com", "adminFirstName", "adminLastName", "qwerty123", 1 },
                    { 2, "chipper@simbirsoft.com", "chipperFirstName", "chipperLastName", "qwerty123", 2 },
                    { 3, "user@simbirsoft.com", "userFirstName", "userLastName", "qwerty123", 4 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Accounts");
        }
    }
}
