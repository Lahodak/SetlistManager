using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.API.Migrations;

/// <inheritdoc />
public partial class help : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_RoomSetlist_Setlists_SetlistId",
            table: "RoomSetlist");

        migrationBuilder.RenameColumn(
            name: "SetlistId",
            table: "RoomSetlist",
            newName: "SetlistsId");

        migrationBuilder.RenameIndex(
            name: "IX_RoomSetlist_SetlistId",
            table: "RoomSetlist",
            newName: "IX_RoomSetlist_SetlistsId");

        migrationBuilder.AddForeignKey(
            name: "FK_RoomSetlist_Setlists_SetlistsId",
            table: "RoomSetlist",
            column: "SetlistsId",
            principalTable: "Setlists",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_RoomSetlist_Setlists_SetlistsId",
            table: "RoomSetlist");

        migrationBuilder.RenameColumn(
            name: "SetlistsId",
            table: "RoomSetlist",
            newName: "SetlistId");

        migrationBuilder.RenameIndex(
            name: "IX_RoomSetlist_SetlistsId",
            table: "RoomSetlist",
            newName: "IX_RoomSetlist_SetlistId");

        migrationBuilder.AddForeignKey(
            name: "FK_RoomSetlist_Setlists_SetlistId",
            table: "RoomSetlist",
            column: "SetlistId",
            principalTable: "Setlists",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
