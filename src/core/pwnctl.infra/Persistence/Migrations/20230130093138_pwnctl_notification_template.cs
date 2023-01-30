using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class pwnctl_notification_template : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Template",
                table: "NotificationRules",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Template",
                table: "NotificationRules");
        }
    }
}
