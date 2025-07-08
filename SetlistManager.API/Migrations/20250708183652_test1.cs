using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.API.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomSetlist");

            migrationBuilder.DropTable(
                name: "SetlistSong");

            migrationBuilder.AddColumn<int>(
                name: "SetlistId",
                table: "Songs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetlistId",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomsSetlists",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    SetlistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomsSetlists", x => new { x.SetlistId, x.RoomId });
                    table.ForeignKey(
                        name: "FK_RoomsSetlists_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomsSetlists_Setlists_SetlistId",
                        column: x => x.SetlistId,
                        principalTable: "Setlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SongsSetlists",
                columns: table => new
                {
                    SongId = table.Column<int>(type: "int", nullable: false),
                    SetlistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongsSetlists", x => new { x.SetlistId, x.SongId });
                    table.ForeignKey(
                        name: "FK_SongsSetlists_Setlists_SetlistId",
                        column: x => x.SetlistId,
                        principalTable: "Setlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SongsSetlists_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_SetlistId",
                table: "Songs",
                column: "SetlistId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_SetlistId",
                table: "Rooms",
                column: "SetlistId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomsSetlists_RoomId",
                table: "RoomsSetlists",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_SongsSetlists_SongId",
                table: "SongsSetlists",
                column: "SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Setlists_SetlistId",
                table: "Rooms",
                column: "SetlistId",
                principalTable: "Setlists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Setlists_SetlistId",
                table: "Songs",
                column: "SetlistId",
                principalTable: "Setlists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Setlists_SetlistId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Setlists_SetlistId",
                table: "Songs");

            migrationBuilder.DropTable(
                name: "RoomsSetlists");

            migrationBuilder.DropTable(
                name: "SongsSetlists");

            migrationBuilder.DropIndex(
                name: "IX_Songs_SetlistId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_SetlistId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "SetlistId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "SetlistId",
                table: "Rooms");

            migrationBuilder.CreateTable(
                name: "RoomSetlist",
                columns: table => new
                {
                    RoomsId = table.Column<int>(type: "int", nullable: false),
                    SetlistsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomSetlist", x => new { x.RoomsId, x.SetlistsId });
                    table.ForeignKey(
                        name: "FK_RoomSetlist_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomSetlist_Setlists_SetlistsId",
                        column: x => x.SetlistsId,
                        principalTable: "Setlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetlistSong",
                columns: table => new
                {
                    SetlistsId = table.Column<int>(type: "int", nullable: false),
                    SongsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetlistSong", x => new { x.SetlistsId, x.SongsId });
                    table.ForeignKey(
                        name: "FK_SetlistSong_Setlists_SetlistsId",
                        column: x => x.SetlistsId,
                        principalTable: "Setlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetlistSong_Songs_SongsId",
                        column: x => x.SongsId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomSetlist_SetlistsId",
                table: "RoomSetlist",
                column: "SetlistsId");

            migrationBuilder.CreateIndex(
                name: "IX_SetlistSong_SongsId",
                table: "SetlistSong",
                column: "SongsId");
        }
    }
}
