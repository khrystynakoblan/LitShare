using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitShare.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDealCompletedToPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deal_completed",
                table: "posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deal_completed",
                table: "posts");
        }
    }
}
