using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class task_def_stdin_query : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "task_profiles",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_task_profiles_ShortName",
                table: "task_profiles",
                newName: "IX_task_profiles_Name");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "scope_aggregates",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_scope_aggregates_ShortName",
                table: "scope_aggregates",
                newName: "IX_scope_aggregates_Name");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "operations",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_operations_ShortName",
                table: "operations",
                newName: "IX_operations_Name");

            migrationBuilder.AddColumn<string>(
                name: "StdinQuery",
                table: "task_definitions",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StdinQuery",
                table: "task_definitions");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "task_profiles",
                newName: "ShortName");

            migrationBuilder.RenameIndex(
                name: "IX_task_profiles_Name",
                table: "task_profiles",
                newName: "IX_task_profiles_ShortName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "scope_aggregates",
                newName: "ShortName");

            migrationBuilder.RenameIndex(
                name: "IX_scope_aggregates_Name",
                table: "scope_aggregates",
                newName: "IX_scope_aggregates_ShortName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "operations",
                newName: "ShortName");

            migrationBuilder.RenameIndex(
                name: "IX_operations_Name",
                table: "operations",
                newName: "IX_operations_ShortName");
        }
    }
}
