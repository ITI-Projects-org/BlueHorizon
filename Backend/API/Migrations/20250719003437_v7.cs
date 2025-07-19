using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId1",
                table: "OwnerVerificationDocuments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OwnerVerificationDocuments_OwnerId1",
                table: "OwnerVerificationDocuments",
                column: "OwnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerVerificationDocuments_AspNetUsers_OwnerId1",
                table: "OwnerVerificationDocuments",
                column: "OwnerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerVerificationDocuments_AspNetUsers_OwnerId1",
                table: "OwnerVerificationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_OwnerVerificationDocuments_OwnerId1",
                table: "OwnerVerificationDocuments");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "OwnerVerificationDocuments");
        }
    }
}
