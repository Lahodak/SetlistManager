using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class renamedtempauthstoragecolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TempSalt",
                table: "TempAuthStorage",
                newName: "TempSecret");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TempSecret",
                table: "TempAuthStorage",
                newName: "TempSalt");
        }
    }
}
