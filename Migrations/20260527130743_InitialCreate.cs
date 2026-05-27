using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebWerverPart.Models;

#nullable disable

namespace WebWerverPart.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:user_status", "admin,standart");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    user_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    user_role = table.Column<UserStatus>(type: "user_status", nullable: false, defaultValue: UserStatus.Standart)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id_user);
                });

            migrationBuilder.CreateTable(
                name: "bug_report",
                columns: table => new
                {
                    id_report = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    report_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bug_report_pkey", x => x.id_report);
                    table.ForeignKey(
                        name: "bug_report_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("refresh_tokens_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bug_report_user_id",
                table: "bug_report",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "refresh_tokens_token_key",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_user_email_key",
                table: "users",
                column: "user_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_user_login_key",
                table: "users",
                column: "user_login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bug_report");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
