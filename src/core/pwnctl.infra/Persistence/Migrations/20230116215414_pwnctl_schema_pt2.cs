using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class pwnctl_schema_pt2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HostId",
                table: "Endpoints",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_HostId",
                table: "Endpoints",
                column: "HostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoints_Hosts_HostId",
                table: "Endpoints",
                column: "HostId",
                principalTable: "Hosts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoints_Hosts_HostId",
                table: "Endpoints");

            migrationBuilder.DropIndex(
                name: "IX_Endpoints_HostId",
                table: "Endpoints");

            migrationBuilder.DropColumn(
                name: "HostId",
                table: "Endpoints");
        }
    }
}
