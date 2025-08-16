using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.API.Migrations
{
    /// <inheritdoc />
    public partial class IntrumentFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstrumentUser");

            migrationBuilder.AddColumn<int>(
                name: "InstrumentId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_InstrumentId",
                table: "Users",
                column: "InstrumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Instruments_InstrumentId",
                table: "Users",
                column: "InstrumentId",
                principalTable: "Instruments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Instruments_InstrumentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_InstrumentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InstrumentId",
                table: "Users");

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

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentUser_UsersId",
                table: "InstrumentUser",
                column: "UsersId");
        }
    }
}
