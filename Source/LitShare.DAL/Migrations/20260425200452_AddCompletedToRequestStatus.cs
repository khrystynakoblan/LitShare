using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitShare.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedToRequestStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TYPE request_status_t ADD VALUE IF NOT EXISTS 'completed';");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:deal_type_t.deal_type", "exchange,donation")
                .Annotation("Npgsql:Enum:request_status", "pending,accepted,rejected,completed")
                .Annotation("Npgsql:Enum:role_t.role_type", "user,admin")
                .OldAnnotation("Npgsql:Enum:deal_type_t.deal_type", "exchange,donation")
                .OldAnnotation("Npgsql:Enum:request_status", "pending,accepted,rejected")
                .OldAnnotation("Npgsql:Enum:role_t.role_type", "user,admin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:deal_type_t.deal_type", "exchange,donation")
                .Annotation("Npgsql:Enum:request_status", "pending,accepted,rejected")
                .Annotation("Npgsql:Enum:role_t.role_type", "user,admin")
                .OldAnnotation("Npgsql:Enum:deal_type_t.deal_type", "exchange,donation")
                .OldAnnotation("Npgsql:Enum:request_status", "pending,accepted,rejected,completed")
                .OldAnnotation("Npgsql:Enum:role_t.role_type", "user,admin");
        }
    }
}
