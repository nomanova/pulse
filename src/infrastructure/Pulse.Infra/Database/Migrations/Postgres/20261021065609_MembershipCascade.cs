using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.Infra.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class MembershipCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_memberships_applications_application_id",
                table: "memberships");

            migrationBuilder.DropForeignKey(
                name: "fk_memberships_environments_environment_id",
                table: "memberships");

            migrationBuilder.DropForeignKey(
                name: "fk_memberships_organizations_organization_id",
                table: "memberships");

            migrationBuilder.AddForeignKey(
                name: "fk_memberships_applications_application_id",
                table: "memberships",
                column: "application_id",
                principalTable: "applications",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_memberships_environments_environment_id",
                table: "memberships",
                column: "environment_id",
                principalTable: "environments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_memberships_organizations_organization_id",
                table: "memberships",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_memberships_applications_application_id",
                table: "memberships");

            migrationBuilder.DropForeignKey(
                name: "fk_memberships_environments_environment_id",
                table: "memberships");

            migrationBuilder.DropForeignKey(
                name: "fk_memberships_organizations_organization_id",
                table: "memberships");

            migrationBuilder.AddForeignKey(
                name: "fk_memberships_applications_application_id",
                table: "memberships",
                column: "application_id",
                principalTable: "applications",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_memberships_environments_environment_id",
                table: "memberships",
                column: "environment_id",
                principalTable: "environments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_memberships_organizations_organization_id",
                table: "memberships",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id");
        }
    }
}
