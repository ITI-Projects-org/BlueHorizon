using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Village_System.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Unit_UnitId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Unit_AspNetUsers_OwnerId",
                table: "Unit");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitAmenities_Unit_UnitId",
                table: "UnitAmenities");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitReviews_Unit_UnitId",
                table: "UnitReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Unit",
                table: "Unit");

            migrationBuilder.RenameTable(
                name: "Unit",
                newName: "Units");

            migrationBuilder.RenameIndex(
                name: "IX_Unit_OwnerId",
                table: "Units",
                newName: "IX_Units_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Units",
                table: "Units",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Units_UnitId",
                table: "Bookings",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitAmenities_Units_UnitId",
                table: "UnitAmenities",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitReviews_Units_UnitId",
                table: "UnitReviews",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_AspNetUsers_OwnerId",
                table: "Units",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Units_UnitId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitAmenities_Units_UnitId",
                table: "UnitAmenities");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitReviews_Units_UnitId",
                table: "UnitReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_AspNetUsers_OwnerId",
                table: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Units",
                table: "Units");

            migrationBuilder.RenameTable(
                name: "Units",
                newName: "Unit");

            migrationBuilder.RenameIndex(
                name: "IX_Units_OwnerId",
                table: "Unit",
                newName: "IX_Unit_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Unit",
                table: "Unit",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Unit_UnitId",
                table: "Bookings",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Unit_AspNetUsers_OwnerId",
                table: "Unit",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitAmenities_Unit_UnitId",
                table: "UnitAmenities",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitReviews_Unit_UnitId",
                table: "UnitReviews",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
