using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetlistManager.Data.Migrations;

/// <inheritdoc />
public partial class RenamedFriendshipColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Friendships_AspNetUsers_User1Id",
            table: "Friendships");

        migrationBuilder.DropForeignKey(
            name: "FK_Friendships_AspNetUsers_User2Id",
            table: "Friendships");

        migrationBuilder.RenameColumn(
            name: "User2Id",
            table: "Friendships",
            newName: "RecieverId");

        migrationBuilder.RenameColumn(
            name: "User1Id",
            table: "Friendships",
            newName: "InitiatorId");

        migrationBuilder.RenameIndex(
            name: "IX_Friendships_User2Id",
            table: "Friendships",
            newName: "IX_Friendships_RecieverId");

        migrationBuilder.RenameIndex(
            name: "IX_Friendships_User1Id",
            table: "Friendships",
            newName: "IX_Friendships_InitiatorId");

        migrationBuilder.AddForeignKey(
            name: "FK_Friendships_AspNetUsers_InitiatorId",
            table: "Friendships",
            column: "InitiatorId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Friendships_AspNetUsers_RecieverId",
            table: "Friendships",
            column: "RecieverId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Friendships_AspNetUsers_InitiatorId",
            table: "Friendships");

        migrationBuilder.DropForeignKey(
            name: "FK_Friendships_AspNetUsers_RecieverId",
            table: "Friendships");

        migrationBuilder.RenameColumn(
            name: "RecieverId",
            table: "Friendships",
            newName: "User2Id");

        migrationBuilder.RenameColumn(
            name: "InitiatorId",
            table: "Friendships",
            newName: "User1Id");

        migrationBuilder.RenameIndex(
            name: "IX_Friendships_RecieverId",
            table: "Friendships",
            newName: "IX_Friendships_User2Id");

        migrationBuilder.RenameIndex(
            name: "IX_Friendships_InitiatorId",
            table: "Friendships",
            newName: "IX_Friendships_User1Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Friendships_AspNetUsers_User1Id",
            table: "Friendships",
            column: "User1Id",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Friendships_AspNetUsers_User2Id",
            table: "Friendships",
            column: "User2Id",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }
}
