using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiChatBot.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationAndAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId1",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ChatMessages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateUtc",
                table: "ChatMessages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ChatMessages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDateUtc",
                table: "ChatMessages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.ConversationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ConversationId1",
                table: "ChatMessages",
                column: "ConversationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId1",
                table: "ChatMessages",
                column: "ConversationId1",
                principalTable: "Conversations",
                principalColumn: "ConversationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId1",
                table: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ConversationId1",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ConversationId1",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "CreatedDateUtc",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedDateUtc",
                table: "ChatMessages");
        }
    }
}
