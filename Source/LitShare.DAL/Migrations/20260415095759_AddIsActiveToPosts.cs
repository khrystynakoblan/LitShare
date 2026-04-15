using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitShare.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "posts",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "posts");
        }
    }
}
