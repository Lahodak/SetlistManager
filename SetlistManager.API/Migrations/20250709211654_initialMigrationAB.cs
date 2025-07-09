using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.API.Migrations
{
    /// <inheritdoc />
    public partial class initialMigrationAB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomsSetlists_Rooms_RoomId",
                table: "RoomsSetlists");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomsSetlists_Setlists_SetlistId",
                table: "RoomsSetlists");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomsSetlists_Rooms_RoomId",
                table: "RoomsSetlists",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomsSetlists_Setlists_SetlistId",
                table: "RoomsSetlists",
                column: "SetlistId",
                principalTable: "Setlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomsSetlists_Rooms_RoomId",
                table: "RoomsSetlists");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomsSetlists_Setlists_SetlistId",
                table: "RoomsSetlists");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomsSetlists_Rooms_RoomId",
                table: "RoomsSetlists",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomsSetlists_Setlists_SetlistId",
                table: "RoomsSetlists",
                column: "SetlistId",
                principalTable: "Setlists",
                principalColumn: "Id");
        }
    }
}
