using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.Infra.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class Normalize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_environments_organizations_organization_id",
                table: "environments");

            migrationBuilder.DropForeignKey(
                name: "fk_workflow_instances_applications_application_id",
                table: "workflow_instances");

            migrationBuilder.DropForeignKey(
                name: "fk_workflow_instances_environments_environment_id",
                table: "workflow_instances");

            migrationBuilder.DropForeignKey(
                name: "fk_workflow_instances_organizations_organization_id",
                table: "workflow_instances");

            migrationBuilder.DropForeignKey(
                name: "fk_workflow_instances_workflows_workflow_id",
                table: "workflow_instances");

            migrationBuilder.DropForeignKey(
                name: "fk_workflows_applications_application_id",
                table: "workflows");

            migrationBuilder.DropForeignKey(
                name: "fk_workflows_organizations_organization_id",
                table: "workflows");

            migrationBuilder.DropIndex(
                name: "ix_workflows_application_id",
                table: "workflows");

            migrationBuilder.DropIndex(
                name: "ix_workflows_organization_id",
                table: "workflows");

            migrationBuilder.DropIndex(
                name: "ix_workflow_instances_application_id",
                table: "workflow_instances");

            migrationBuilder.DropIndex(
                name: "ix_workflow_instances_environment_id",
                table: "workflow_instances");

            migrationBuilder.DropIndex(
                name: "ix_workflow_instances_organization_id",
                table: "workflow_instances");

            migrationBuilder.DropIndex(
                name: "ix_workflow_instances_workflow_id",
                table: "workflow_instances");

            migrationBuilder.DropIndex(
                name: "ix_environments_organization_id",
                table: "environments");

            migrationBuilder.DropColumn(
                name: "application_id",
                table: "workflows");

            migrationBuilder.DropColumn(
                name: "organization_id",
                table: "workflows");

            migrationBuilder.DropColumn(
                name: "application_id",
                table: "workflow_instances");

            migrationBuilder.DropColumn(
                name: "environment_id",
                table: "workflow_instances");

            migrationBuilder.DropColumn(
                name: "organization_id",
                table: "workflow_instances");

            migrationBuilder.DropColumn(
                name: "workflow_id",
                table: "workflow_instances");

            migrationBuilder.DropColumn(
                name: "organization_id",
                table: "environments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "application_id",
                table: "workflows",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "organization_id",
                table: "workflows",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "application_id",
                table: "workflow_instances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "environment_id",
                table: "workflow_instances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "organization_id",
                table: "workflow_instances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "workflow_id",
                table: "workflow_instances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "organization_id",
                table: "environments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_workflows_application_id",
                table: "workflows",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflows_organization_id",
                table: "workflows",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_instances_application_id",
                table: "workflow_instances",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_instances_environment_id",
                table: "workflow_instances",
                column: "environment_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_instances_organization_id",
                table: "workflow_instances",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_instances_workflow_id",
                table: "workflow_instances",
                column: "workflow_id");

            migrationBuilder.CreateIndex(
                name: "ix_environments_organization_id",
                table: "environments",
                column: "organization_id");

            migrationBuilder.AddForeignKey(
                name: "fk_environments_organizations_organization_id",
                table: "environments",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workflow_instances_applications_application_id",
                table: "workflow_instances",
                column: "application_id",
                principalTable: "applications",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workflow_instances_environments_environment_id",
                table: "workflow_instances",
                column: "environment_id",
                principalTable: "environments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workflow_instances_organizations_organization_id",
                table: "workflow_instances",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workflow_instances_workflows_workflow_id",
                table: "workflow_instances",
                column: "workflow_id",
                principalTable: "workflows",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workflows_applications_application_id",
                table: "workflows",
                column: "application_id",
                principalTable: "applications",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workflows_organizations_organization_id",
                table: "workflows",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
