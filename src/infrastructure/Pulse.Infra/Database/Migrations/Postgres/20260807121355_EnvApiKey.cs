using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.Infra.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class EnvApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pri_api_key",
                table: "environments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sec_api_key",
                table: "environments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pri_api_key",
                table: "environments");

            migrationBuilder.DropColumn(
                name: "sec_api_key",
                table: "environments");
        }
    }
}
