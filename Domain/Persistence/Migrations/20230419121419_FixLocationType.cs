using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Domain.Persistence.Migrations
{
    public partial class FixLocationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "LocationPoints");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "LocationPoints");

            migrationBuilder.AddColumn<Point>(
                name: "Point",
                table: "LocationPoints",
                type: "geometry",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Point",
                table: "LocationPoints");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "LocationPoints",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "LocationPoints",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
