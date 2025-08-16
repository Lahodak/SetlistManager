using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.API.Migrations
{
    /// <inheritdoc />
    public partial class skibidinininini : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Setlists_SetlistId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_SetlistId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "SetlistId",
                table: "Songs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SetlistId",
                table: "Songs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Songs_SetlistId",
                table: "Songs",
                column: "SetlistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Setlists_SetlistId",
                table: "Songs",
                column: "SetlistId",
                principalTable: "Setlists",
                principalColumn: "Id");
        }
    }
}
