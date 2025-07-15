using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Village_System.Migrations
{
    /// <inheritdoc />
    public partial class The_Complete_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankAccountDetails",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationNotes",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccountDetails",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VerificationDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VerificationNotes",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "AspNetUsers");
        }
    }
}
