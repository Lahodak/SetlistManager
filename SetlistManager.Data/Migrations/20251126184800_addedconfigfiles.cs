using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedconfigfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Rooms_RoomId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Setlists_SetlistId",
                table: "Rooms");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Rooms_RoomId",
                table: "AspNetUsers",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Setlists_SetlistId",
                table: "Rooms",
                column: "SetlistId",
                principalTable: "Setlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Rooms_RoomId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Setlists_SetlistId",
                table: "Rooms");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Rooms_RoomId",
                table: "AspNetUsers",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Setlists_SetlistId",
                table: "Rooms",
                column: "SetlistId",
                principalTable: "Setlists",
                principalColumn: "Id");
        }
    }
}
