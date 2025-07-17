using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Village_System.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitAmenities_Units_UnitId",
                table: "UnitAmenities");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitAmenities_Units_UnitId",
                table: "UnitAmenities",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitAmenities_Units_UnitId",
                table: "UnitAmenities");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitAmenities_Units_UnitId",
                table: "UnitAmenities",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
