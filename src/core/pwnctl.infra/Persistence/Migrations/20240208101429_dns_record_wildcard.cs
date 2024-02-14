using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class dns_record_wildcard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Wildcard",
                table: "domain_name_records",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Wildcard",
                table: "domain_name_records");
        }
    }
}
