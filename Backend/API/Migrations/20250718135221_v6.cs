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
                name: "FK_Messages_AspNetUsers_Reciever",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_Sender",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Sender",
                table: "Messages",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "Reciever",
                table: "Messages",
                newName: "RecieverId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_Sender",
                table: "Messages",
                newName: "IX_Messages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_Reciever",
                table: "Messages",
                newName: "IX_Messages_RecieverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_RecieverId",
                table: "Messages",
                column: "RecieverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_RecieverId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Messages",
                newName: "Sender");

            migrationBuilder.RenameColumn(
                name: "RecieverId",
                table: "Messages",
                newName: "Reciever");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                newName: "IX_Messages_Sender");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_RecieverId",
                table: "Messages",
                newName: "IX_Messages_Reciever");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_Reciever",
                table: "Messages",
                column: "Reciever",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_Sender",
                table: "Messages",
                column: "Sender",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
