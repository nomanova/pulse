using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.Infra.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Manually added
            migrationBuilder.CreateIndex(
                name: "ix_applications_organization_id_key",
                table: "applications",
                columns: new[] { "organization_id", "key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_environments_application_id_key",
                table: "environments",
                columns: new[] { "application_id", "key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_workflows_environment_id_key",
                table: "workflows",
                columns: new[] { "environment_id", "key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Manually added
            migrationBuilder.DropIndex(
                name: "ix_applications_organization_id_key",
                table: "applications");
            
            migrationBuilder.DropIndex(
                name: "ix_environments_application_id_key",
                table: "environments");
            
            migrationBuilder.DropIndex(
                name: "ix_workflows_environment_id_key",
                table: "workflows");
        }
    }
}