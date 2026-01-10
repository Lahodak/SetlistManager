using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.Data.Migrations;

/// <inheritdoc />
public partial class AddedFriendships : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Friendships",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                User1Id = table.Column<int>(type: "int", nullable: false),
                User2Id = table.Column<int>(type: "int", nullable: false),
                State = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Friendships", x => x.Id);
                table.ForeignKey(
                    name: "FK_Friendships_AspNetUsers_User1Id",
                    column: x => x.User1Id,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Friendships_AspNetUsers_User2Id",
                    column: x => x.User2Id,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Friendships_User1Id",
            table: "Friendships",
            column: "User1Id");

        migrationBuilder.CreateIndex(
            name: "IX_Friendships_User2Id",
            table: "Friendships",
            column: "User2Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Friendships");
    }
}
