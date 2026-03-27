using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserPropertyToTempAuthStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TempAuthStorage_UserId",
                table: "TempAuthStorage",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TempAuthStorage_AspNetUsers_UserId",
                table: "TempAuthStorage",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TempAuthStorage_AspNetUsers_UserId",
                table: "TempAuthStorage");

            migrationBuilder.DropIndex(
                name: "IX_TempAuthStorage_UserId",
                table: "TempAuthStorage");
        }
    }
}
