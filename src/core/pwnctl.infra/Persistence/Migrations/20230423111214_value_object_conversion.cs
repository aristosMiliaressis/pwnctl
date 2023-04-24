using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class value_object_conversion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortName_Value",
                table: "task_profiles",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "SubjectClass_Value",
                table: "task_definitions",
                newName: "SubjectClass");

            migrationBuilder.RenameColumn(
                name: "ShortName_Value",
                table: "task_definitions",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "ShortName_Value",
                table: "scope_aggregates",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "ShortName_Value",
                table: "operations",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "SubjectClass_Value",
                table: "notification_rules",
                newName: "SubjectClass");

            migrationBuilder.RenameColumn(
                name: "ShortName_Value",
                table: "notification_rules",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "SubjectClass_Value",
                table: "asset_records",
                newName: "SubjectClass");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "task_profiles",
                newName: "ShortName_Value");

            migrationBuilder.RenameColumn(
                name: "SubjectClass",
                table: "task_definitions",
                newName: "SubjectClass_Value");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "task_definitions",
                newName: "ShortName_Value");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "scope_aggregates",
                newName: "ShortName_Value");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "operations",
                newName: "ShortName_Value");

            migrationBuilder.RenameColumn(
                name: "SubjectClass",
                table: "notification_rules",
                newName: "SubjectClass_Value");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "notification_rules",
                newName: "ShortName_Value");

            migrationBuilder.RenameColumn(
                name: "SubjectClass",
                table: "asset_records",
                newName: "SubjectClass_Value");
        }
    }
}
