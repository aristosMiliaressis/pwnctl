using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class removed_root_domain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RootDomain",
                table: "domain_names");

            migrationBuilder.AddColumn<string>(
                name: "RootDomain",
                table: "http_endpoints",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RootDomain",
                table: "http_endpoints");

            migrationBuilder.AddColumn<string>(
                name: "RootDomain",
                table: "domain_names",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
