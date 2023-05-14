using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class value_object : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubjectClass",
                table: "task_definitions",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "task_definitions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SubjectClass",
                table: "notification_rules",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "notification_rules",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_notification_rules_ShortName",
                table: "notification_rules",
                newName: "IX_notification_rules_Name");

            migrationBuilder.RenameColumn(
                name: "SubjectClass",
                table: "asset_records",
                newName: "Subject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "task_definitions",
                newName: "SubjectClass");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "task_definitions",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "notification_rules",
                newName: "SubjectClass");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "notification_rules",
                newName: "ShortName");

            migrationBuilder.RenameIndex(
                name: "IX_notification_rules_Name",
                table: "notification_rules",
                newName: "IX_notification_rules_ShortName");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "asset_records",
                newName: "SubjectClass");
        }
    }
}
