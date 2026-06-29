using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.Infra.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class Messaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inbox_messages",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    handler = table.Column<string>(type: "text", nullable: false),
                    received_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_messages", x => new { x.id, x.handler });
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "jsonb", nullable: false),
                    occurred_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processing_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    processing_id = table.Column<string>(type: "text", nullable: true),
                    processed_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_inbox_messages_processed_on",
                table: "inbox_messages",
                column: "processed_on");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_occurred_on",
                table: "outbox_messages",
                column: "occurred_on",
                filter: "processed_on IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_on",
                table: "outbox_messages",
                column: "processed_on",
                filter: "processed_on IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inbox_messages");

            migrationBuilder.DropTable(
                name: "outbox_messages");
        }
    }
}
