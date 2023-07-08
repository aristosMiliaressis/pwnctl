using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class task_def_improvements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CheckNotificationRules",
                table: "task_definitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckNotificationRules",
                table: "task_definitions");
        }
    }
}
