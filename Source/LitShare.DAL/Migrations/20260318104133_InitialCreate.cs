using System;
using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LitShare.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE TYPE role_t AS ENUM ('user','admin');");
            migrationBuilder.Sql("CREATE TYPE deal_type_t AS ENUM ('exchange','donation');");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:deal_type_t.deal_type", "exchange,donation")
                .Annotation("Npgsql:Enum:role_t.role_type", "user,admin");

            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    hashedPassword = table.Column<string>(type: "text", nullable: false),
                    about = table.Column<string>(type: "text", nullable: true),
                    region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    role = table.Column<RoleType>(type: "role_t", nullable: false, defaultValue: RoleType.User),
                    photo_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    author = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    deal_type = table.Column<DealType>(type: "deal_type_t", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    photo_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.id);
                    table.ForeignKey(
                        name: "FK_posts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "books_genres",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    genre_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books_genres", x => new { x.post_id, x.genre_id });
                    table.ForeignKey(
                        name: "FK_books_genres_genres_genre_id",
                        column: x => x.genre_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_books_genres_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "complaints",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    complainant_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_complaints", x => x.id);
                    table.ForeignKey(
                        name: "FK_complaints_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_complaints_users_complainant_id",
                        column: x => x.complainant_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_books_genres_genre_id",
                table: "books_genres",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_complainant_id",
                table: "complaints",
                column: "complainant_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_post_id",
                table: "complaints",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_posts_user_id",
                table: "posts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "books_genres");

            migrationBuilder.DropTable(
                name: "complaints");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
