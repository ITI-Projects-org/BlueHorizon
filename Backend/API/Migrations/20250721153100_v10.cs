using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class v10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocumentPath",
                table: "OwnerVerificationDocuments",
                newName: "FrontNationalIdDocumentPath");

            migrationBuilder.AddColumn<string>(
                name: "BackNationalIdDocumentPath",
                table: "OwnerVerificationDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackNationalIdDocumentPath",
                table: "OwnerVerificationDocuments");

            migrationBuilder.RenameColumn(
                name: "FrontNationalIdDocumentPath",
                table: "OwnerVerificationDocuments",
                newName: "DocumentPath");
        }
    }
}
