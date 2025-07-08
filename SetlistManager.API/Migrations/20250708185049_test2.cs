using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.API.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
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

            migrationBuilder.DropForeignKey(
                name: "FK_SongsSetlists_Setlists_SetlistId",
                table: "SongsSetlists");

            migrationBuilder.DropForeignKey(
                name: "FK_SongsSetlists_Songs_SongId",
                table: "SongsSetlists");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomsSetlists_Rooms_RoomId",
                table: "RoomsSetlists",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomsSetlists_Setlists_SetlistId",
                table: "RoomsSetlists",
                column: "SetlistId",
                principalTable: "Setlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongsSetlists_Setlists_SetlistId",
                table: "SongsSetlists",
                column: "SetlistId",
                principalTable: "Setlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongsSetlists_Songs_SongId",
                table: "SongsSetlists",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.DropForeignKey(
                name: "FK_SongsSetlists_Setlists_SetlistId",
                table: "SongsSetlists");

            migrationBuilder.DropForeignKey(
                name: "FK_SongsSetlists_Songs_SongId",
                table: "SongsSetlists");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SongsSetlists_Setlists_SetlistId",
                table: "SongsSetlists",
                column: "SetlistId",
                principalTable: "Setlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SongsSetlists_Songs_SongId",
                table: "SongsSetlists",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
