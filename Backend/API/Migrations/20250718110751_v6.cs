using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_AspNetUsers_OwnerId",
                table: "Units");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_AspNetUsers_OwnerId1",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_OwnerId1",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "Units");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_AspNetUsers_OwnerId",
                table: "Units",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_AspNetUsers_OwnerId",
                table: "Units");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId1",
                table: "Units",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_OwnerId1",
                table: "Units",
                column: "OwnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_AspNetUsers_OwnerId",
                table: "Units",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_AspNetUsers_OwnerId1",
                table: "Units",
                column: "OwnerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
