using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiChatBot.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConversationIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId1",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ConversationId1",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ConversationId1",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversationId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ConversationId",
                table: "ChatMessages",
                column: "ConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ConversationId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "ConversationId",
                table: "ChatMessages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId1",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: true);

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
    }
}
