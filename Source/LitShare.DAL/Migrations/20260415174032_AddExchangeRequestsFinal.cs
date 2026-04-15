using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitShare.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddExchangeRequestsFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exchange_requests_posts_PostId",
                table: "exchange_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_exchange_requests_users_SenderId",
                table: "exchange_requests");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "exchange_requests",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "exchange_requests",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "exchange_requests",
                newName: "sender_id");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "exchange_requests",
                newName: "post_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "exchange_requests",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_exchange_requests_SenderId_PostId",
                table: "exchange_requests",
                newName: "IX_exchange_requests_sender_id_post_id");

            migrationBuilder.RenameIndex(
                name: "IX_exchange_requests_PostId",
                table: "exchange_requests",
                newName: "IX_exchange_requests_post_id");

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_requests_posts_post_id",
                table: "exchange_requests",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_requests_users_sender_id",
                table: "exchange_requests",
                column: "sender_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exchange_requests_posts_post_id",
                table: "exchange_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_exchange_requests_users_sender_id",
                table: "exchange_requests");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "exchange_requests",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "exchange_requests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sender_id",
                table: "exchange_requests",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "post_id",
                table: "exchange_requests",
                newName: "PostId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "exchange_requests",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_exchange_requests_sender_id_post_id",
                table: "exchange_requests",
                newName: "IX_exchange_requests_SenderId_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_exchange_requests_post_id",
                table: "exchange_requests",
                newName: "IX_exchange_requests_PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_requests_posts_PostId",
                table: "exchange_requests",
                column: "PostId",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_requests_users_SenderId",
                table: "exchange_requests",
                column: "SenderId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
