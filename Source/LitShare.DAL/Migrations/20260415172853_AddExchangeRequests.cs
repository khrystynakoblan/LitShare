using System;
using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LitShare.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddExchangeRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:deal_type_t.deal_type", "exchange,donation")
                .Annotation("Npgsql:Enum:request_status", "pending,accepted,rejected")
                .Annotation("Npgsql:Enum:role_t.role_type", "user,admin")
                .OldAnnotation("Npgsql:Enum:deal_type_t.deal_type", "exchange,donation")
                .OldAnnotation("Npgsql:Enum:role_t.role_type", "user,admin");

            migrationBuilder.CreateTable(
                name: "exchange_requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    PostId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<RequestStatus>(type: "request_status_t", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchange_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exchange_requests_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exchange_requests_users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_exchange_requests_PostId",
                table: "exchange_requests",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_requests_SenderId_PostId",
                table: "exchange_requests",
                columns: new[] { "SenderId", "PostId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exchange_requests");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:deal_type_t.deal_type", "exchange,donation")
                .Annotation("Npgsql:Enum:role_t.role_type", "user,admin")
                .OldAnnotation("Npgsql:Enum:deal_type_t.deal_type", "exchange,donation")
                .OldAnnotation("Npgsql:Enum:request_status", "pending,accepted,rejected")
                .OldAnnotation("Npgsql:Enum:role_t.role_type", "user,admin");
        }
    }
}
