using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnwrk.infra.Persistence.Migrations
{
    public partial class removed_unnecesary_props : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "OperationalPolicies");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "NotificationRules");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OperationalPolicies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "NotificationRules",
                type: "text",
                nullable: true);
        }
    }
}
