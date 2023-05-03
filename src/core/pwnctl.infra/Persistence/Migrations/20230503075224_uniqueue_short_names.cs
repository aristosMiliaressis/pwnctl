using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class uniqueue_short_names : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_task_profiles_ShortName",
                table: "task_profiles",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scope_aggregates_ShortName",
                table: "scope_aggregates",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_operations_ShortName",
                table: "operations",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_rules_ShortName",
                table: "notification_rules",
                column: "ShortName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_task_profiles_ShortName",
                table: "task_profiles");

            migrationBuilder.DropIndex(
                name: "IX_scope_aggregates_ShortName",
                table: "scope_aggregates");

            migrationBuilder.DropIndex(
                name: "IX_operations_ShortName",
                table: "operations");

            migrationBuilder.DropIndex(
                name: "IX_notification_rules_ShortName",
                table: "notification_rules");
        }
    }
}
