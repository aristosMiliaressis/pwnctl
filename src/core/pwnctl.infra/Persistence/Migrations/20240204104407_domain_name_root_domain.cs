using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class domain_name_root_domain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RootDomain",
                table: "domain_names",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RootDomain",
                table: "domain_names");
        }
    }
}
