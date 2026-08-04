using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace RemsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedAnalysisGeometry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedAnalysisGeometries",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    PolygonA = table.Column<Geometry>(type: "geometry", nullable: false),
                    PolygonB = table.Column<Geometry>(type: "geometry", nullable: false),
                    PolygonC = table.Column<Geometry>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedAnalysisGeometries", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedAnalysisGeometries");
        }
    }
}
