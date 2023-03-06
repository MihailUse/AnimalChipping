using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class AddIdToVisitedLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimalVisitedLocations",
                table: "AnimalVisitedLocations");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "AnimalVisitedLocations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimalVisitedLocations",
                table: "AnimalVisitedLocations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalVisitedLocations_AnimalId",
                table: "AnimalVisitedLocations",
                column: "AnimalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimalVisitedLocations",
                table: "AnimalVisitedLocations");

            migrationBuilder.DropIndex(
                name: "IX_AnimalVisitedLocations_AnimalId",
                table: "AnimalVisitedLocations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AnimalVisitedLocations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimalVisitedLocations",
                table: "AnimalVisitedLocations",
                columns: new[] { "AnimalId", "LocationPointId" });
        }
    }
}
