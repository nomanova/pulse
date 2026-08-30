using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.Infra.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class StepDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_workflow_instance_steps_workflow_version_steps_workflow_ver",
                table: "workflow_instance_steps");

            migrationBuilder.DropForeignKey(
                name: "fk_workflow_instances_workflow_versions_workflow_version_id",
                table: "workflow_instances");

            migrationBuilder.AddColumn<string>(
                name: "definition",
                table: "workflow_version_steps",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "workflow_version_id",
                table: "workflow_instances",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "environment_id",
                table: "workflow_instances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "source",
                table: "workflow_instances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "workflow_version_step_id",
                table: "workflow_instance_steps",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "definition",
                table: "workflow_instance_steps",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_instances_environment_id",
                table: "workflow_instances",
                column: "environment_id");

            migrationBuilder.AddForeignKey(
                name: "fk_workflow_instance_steps_workflow_version_steps_workflow_ver",
                table: "workflow_instance_steps",
                column: "workflow_version_step_id",
                principalTable: "workflow_version_steps",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_workflow_instances_environments_environment_id",
                table: "workflow_instances",
                column: "environment_id",
                principalTable: "environments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workflow_instances_workflow_versions_workflow_version_id",
                table: "workflow_instances",
                column: "workflow_version_id",
                principalTable: "workflow_versions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_workflow_instance_steps_workflow_version_steps_workflow_ver",
                table: "workflow_instance_steps");

            migrationBuilder.DropForeignKey(
                name: "fk_workflow_instances_environments_environment_id",
                table: "workflow_instances");

            migrationBuilder.DropForeignKey(
                name: "fk_workflow_instances_workflow_versions_workflow_version_id",
                table: "workflow_instances");

            migrationBuilder.DropIndex(
                name: "ix_workflow_instances_environment_id",
                table: "workflow_instances");

            migrationBuilder.DropColumn(
                name: "definition",
                table: "workflow_version_steps");

            migrationBuilder.DropColumn(
                name: "environment_id",
                table: "workflow_instances");

            migrationBuilder.DropColumn(
                name: "source",
                table: "workflow_instances");

            migrationBuilder.DropColumn(
                name: "definition",
                table: "workflow_instance_steps");

            migrationBuilder.AlterColumn<string>(
                name: "workflow_version_id",
                table: "workflow_instances",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "workflow_version_step_id",
                table: "workflow_instance_steps",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_workflow_instance_steps_workflow_version_steps_workflow_ver",
                table: "workflow_instance_steps",
                column: "workflow_version_step_id",
                principalTable: "workflow_version_steps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workflow_instances_workflow_versions_workflow_version_id",
                table: "workflow_instances",
                column: "workflow_version_id",
                principalTable: "workflow_versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
