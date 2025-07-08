using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.API.Migrations;

/// <inheritdoc />
public partial class init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Instruments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Instruments", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Languages",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Languages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Rooms",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                IsPublic = table.Column<bool>(type: "bit", nullable: false),
                HostId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedBy = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Rooms", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Songs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Artist = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TabsURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AudioURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Tuning = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                BPM = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedBy = table.Column<int>(type: "int", nullable: false),
                LanguageId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Songs", x => x.Id);
                table.ForeignKey(
                    name: "FK_Songs_Languages_LanguageId",
                    column: x => x.LanguageId,
                    principalTable: "Languages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                RoomId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
                table.ForeignKey(
                    name: "FK_Users_Rooms_RoomId",
                    column: x => x.RoomId,
                    principalTable: "Rooms",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "InstrumentUser",
            columns: table => new
            {
                InstrumentsId = table.Column<int>(type: "int", nullable: false),
                UsersId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InstrumentUser", x => new { x.InstrumentsId, x.UsersId });
                table.ForeignKey(
                    name: "FK_InstrumentUser_Instruments_InstrumentsId",
                    column: x => x.InstrumentsId,
                    principalTable: "Instruments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_InstrumentUser_Users_UsersId",
                    column: x => x.UsersId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Setlists",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<int>(type: "int", nullable: false),
                UpdatedBy = table.Column<int>(type: "int", nullable: false),
                SongId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Setlists", x => x.Id);
                table.ForeignKey(
                    name: "FK_Setlists_Songs_SongId",
                    column: x => x.SongId,
                    principalTable: "Songs",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Setlists_Users_CreatorId",
                    column: x => x.CreatorId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RoomSetlist",
            columns: table => new
            {
                RoomsId = table.Column<int>(type: "int", nullable: false),
                SetlistId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RoomSetlist", x => new { x.RoomsId, x.SetlistId });
                table.ForeignKey(
                    name: "FK_RoomSetlist_Rooms_RoomsId",
                    column: x => x.RoomsId,
                    principalTable: "Rooms",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RoomSetlist_Setlists_SetlistId",
                    column: x => x.SetlistId,
                    principalTable: "Setlists",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_InstrumentUser_UsersId",
            table: "InstrumentUser",
            column: "UsersId");

        migrationBuilder.CreateIndex(
            name: "IX_RoomSetlist_SetlistId",
            table: "RoomSetlist",
            column: "SetlistId");

        migrationBuilder.CreateIndex(
            name: "IX_Setlists_CreatorId",
            table: "Setlists",
            column: "CreatorId");

        migrationBuilder.CreateIndex(
            name: "IX_Setlists_SongId",
            table: "Setlists",
            column: "SongId");

        migrationBuilder.CreateIndex(
            name: "IX_Songs_LanguageId",
            table: "Songs",
            column: "LanguageId");

        migrationBuilder.CreateIndex(
            name: "IX_Users_RoomId",
            table: "Users",
            column: "RoomId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InstrumentUser");

        migrationBuilder.DropTable(
            name: "RoomSetlist");

        migrationBuilder.DropTable(
            name: "Instruments");

        migrationBuilder.DropTable(
            name: "Setlists");

        migrationBuilder.DropTable(
            name: "Songs");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "Languages");

        migrationBuilder.DropTable(
            name: "Rooms");
    }
}
