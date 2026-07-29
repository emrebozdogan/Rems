using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Properties_NeighborhoodId",
                table: "Properties",
                column: "NeighborhoodId");

            migrationBuilder.CreateIndex(
                name: "IX_Neighborhoods_DistrictId",
                table: "Neighborhoods",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_CityId",
                table: "Districts",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Districts_Cities_CityId",
                table: "Districts",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Neighborhoods_Districts_DistrictId",
                table: "Neighborhoods",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Neighborhoods_NeighborhoodId",
                table: "Properties",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Districts_Cities_CityId",
                table: "Districts");

            migrationBuilder.DropForeignKey(
                name: "FK_Neighborhoods_Districts_DistrictId",
                table: "Neighborhoods");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Neighborhoods_NeighborhoodId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_NeighborhoodId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Neighborhoods_DistrictId",
                table: "Neighborhoods");

            migrationBuilder.DropIndex(
                name: "IX_Districts_CityId",
                table: "Districts");
        }
    }
}
