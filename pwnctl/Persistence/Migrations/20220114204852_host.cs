using Microsoft.EntityFrameworkCore.Migrations;

namespace pwnctl.Migrations
{
    public partial class host : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HostId1",
                table: "DNSRecords",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_HostId1",
                table: "DNSRecords",
                column: "HostId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DNSRecords_Hosts_HostId1",
                table: "DNSRecords",
                column: "HostId1",
                principalTable: "Hosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DNSRecords_Hosts_HostId1",
                table: "DNSRecords");

            migrationBuilder.DropIndex(
                name: "IX_DNSRecords_HostId1",
                table: "DNSRecords");

            migrationBuilder.DropColumn(
                name: "HostId1",
                table: "DNSRecords");
        }
    }
}
