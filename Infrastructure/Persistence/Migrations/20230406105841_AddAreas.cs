using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class AddAreas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:account_role", "admin,chipper,user")
                .Annotation("Npgsql:Enum:animal_gender", "male,female,other")
                .Annotation("Npgsql:Enum:animal_life_status", "alive,dead")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AreaPoints = table.Column<LineString>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:account_role", "admin,chipper,user")
                .OldAnnotation("Npgsql:Enum:animal_gender", "male,female,other")
                .OldAnnotation("Npgsql:Enum:animal_life_status", "alive,dead")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
