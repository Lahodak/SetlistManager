using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.API.Migrations;

/// <inheritdoc />
public partial class FixedInitialError : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Setlists_Songs_SongId",
            table: "Setlists");

        migrationBuilder.DropIndex(
            name: "IX_Setlists_SongId",
            table: "Setlists");

        migrationBuilder.DropColumn(
            name: "SongId",
            table: "Setlists");

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
            name: "IX_SetlistSong_SongsId",
            table: "SetlistSong",
            column: "SongsId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SetlistSong");

        migrationBuilder.AddColumn<int>(
            name: "SongId",
            table: "Setlists",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Setlists_SongId",
            table: "Setlists",
            column: "SongId");

        migrationBuilder.AddForeignKey(
            name: "FK_Setlists_Songs_SongId",
            table: "Setlists",
            column: "SongId",
            principalTable: "Songs",
            principalColumn: "Id");
    }
}
