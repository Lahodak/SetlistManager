using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSharingRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Setlists_AspNetUsers_CreatorId",
                table: "Setlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Artists_ArtistId",
                table: "Songs");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Languages_LanguageId",
                table: "Songs");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Songs",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Setlists",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Setlists_CreatorId",
                table: "Setlists",
                newName: "IX_Setlists_OwnerId");

            migrationBuilder.AlterColumn<string>(
                name: "Tuning",
                table: "Songs",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TabsURL",
                table: "Songs",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "Songs",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AudioURL",
                table: "Songs",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Songs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Nick",
                table: "Artists",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Artists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Artists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ArtistsUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtistId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistsUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistsUsers_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistsUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SetlistsUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SetlistId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetlistsUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetlistsUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SetlistsUsers_Setlists_SetlistId",
                        column: x => x.SetlistId,
                        principalTable: "Setlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SongsUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SongId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongsUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongsUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SongsUsers_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_OwnerId",
                table: "Songs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_OwnerId",
                table: "Artists",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistsUsers_ArtistId",
                table: "ArtistsUsers",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistsUsers_UserId",
                table: "ArtistsUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SetlistsUsers_SetlistId",
                table: "SetlistsUsers",
                column: "SetlistId");

            migrationBuilder.CreateIndex(
                name: "IX_SetlistsUsers_UserId",
                table: "SetlistsUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SongsUsers_SongId",
                table: "SongsUsers",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_SongsUsers_UserId",
                table: "SongsUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_AspNetUsers_OwnerId",
                table: "Artists",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Setlists_AspNetUsers_OwnerId",
                table: "Setlists",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Artists_ArtistId",
                table: "Songs",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_AspNetUsers_OwnerId",
                table: "Songs",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Languages_LanguageId",
                table: "Songs",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_AspNetUsers_OwnerId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_Setlists_AspNetUsers_OwnerId",
                table: "Setlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Artists_ArtistId",
                table: "Songs");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_AspNetUsers_OwnerId",
                table: "Songs");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Languages_LanguageId",
                table: "Songs");

            migrationBuilder.DropTable(
                name: "ArtistsUsers");

            migrationBuilder.DropTable(
                name: "SetlistsUsers");

            migrationBuilder.DropTable(
                name: "SongsUsers");

            migrationBuilder.DropIndex(
                name: "IX_Songs_OwnerId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Artists_OwnerId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Artists");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Songs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Setlists",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Setlists_OwnerId",
                table: "Setlists",
                newName: "IX_Setlists_CreatorId");

            migrationBuilder.AlterColumn<string>(
                name: "Tuning",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "TabsURL",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "AudioURL",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "Nick",
                table: "Artists",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Setlists_AspNetUsers_CreatorId",
                table: "Setlists",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Artists_ArtistId",
                table: "Songs",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Languages_LanguageId",
                table: "Songs",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
