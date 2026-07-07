using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.Infra.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class Workflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workflows",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    organization_id = table.Column<string>(type: "text", nullable: false),
                    application_id = table.Column<string>(type: "text", nullable: false),
                    environment_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    normalized_name = table.Column<string>(type: "text", nullable: false),
                    published_version_id = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflows", x => x.id);
                    table.ForeignKey(
                        name: "fk_workflows_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workflows_environments_environment_id",
                        column: x => x.environment_id,
                        principalTable: "environments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workflows_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workflow_versions",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    workflow_id = table.Column<string>(type: "text", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_versions", x => x.id);
                    table.ForeignKey(
                        name: "fk_workflow_versions_workflows_workflow_id",
                        column: x => x.workflow_id,
                        principalTable: "workflows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workflow_instances",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    organization_id = table.Column<string>(type: "text", nullable: false),
                    application_id = table.Column<string>(type: "text", nullable: false),
                    environment_id = table.Column<string>(type: "text", nullable: false),
                    workflow_id = table.Column<string>(type: "text", nullable: false),
                    workflow_version_id = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_instances", x => x.id);
                    table.ForeignKey(
                        name: "fk_workflow_instances_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workflow_instances_environments_environment_id",
                        column: x => x.environment_id,
                        principalTable: "environments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workflow_instances_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workflow_instances_workflow_versions_workflow_version_id",
                        column: x => x.workflow_version_id,
                        principalTable: "workflow_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workflow_instances_workflows_workflow_id",
                        column: x => x.workflow_id,
                        principalTable: "workflows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workflow_version_steps",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    workflow_version_id = table.Column<string>(type: "text", nullable: false),
                    order = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_version_steps", x => x.id);
                    table.ForeignKey(
                        name: "fk_workflow_version_steps_workflow_versions_workflow_version_id",
                        column: x => x.workflow_version_id,
                        principalTable: "workflow_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workflow_instance_steps",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    workflow_instance_id = table.Column<string>(type: "text", nullable: false),
                    workflow_version_step_id = table.Column<string>(type: "text", nullable: false),
                    order = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    failed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_instance_steps", x => x.id);
                    table.ForeignKey(
                        name: "fk_workflow_instance_steps_workflow_instances_workflow_instanc",
                        column: x => x.workflow_instance_id,
                        principalTable: "workflow_instances",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workflow_instance_steps_workflow_version_steps_workflow_ver",
                        column: x => x.workflow_version_step_id,
                        principalTable: "workflow_version_steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_workflow_instance_steps_workflow_instance_id_order",
                table: "workflow_instance_steps",
                columns: new[] { "workflow_instance_id", "order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_workflow_instance_steps_workflow_version_step_id",
                table: "workflow_instance_steps",
                column: "workflow_version_step_id");

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
                name: "ix_workflow_instances_workflow_version_id",
                table: "workflow_instances",
                column: "workflow_version_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_version_steps_workflow_version_id_order",
                table: "workflow_version_steps",
                columns: new[] { "workflow_version_id", "order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_workflow_versions_workflow_id_version",
                table: "workflow_versions",
                columns: new[] { "workflow_id", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_workflows_application_id",
                table: "workflows",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflows_environment_id",
                table: "workflows",
                column: "environment_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflows_organization_id",
                table: "workflows",
                column: "organization_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workflow_instance_steps");

            migrationBuilder.DropTable(
                name: "workflow_instances");

            migrationBuilder.DropTable(
                name: "workflow_version_steps");

            migrationBuilder.DropTable(
                name: "workflow_versions");

            migrationBuilder.DropTable(
                name: "workflows");
        }
    }
}
