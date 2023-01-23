using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class pwnctl_dnsrecord_index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DNSRecords_Type_Key",
                table: "DNSRecords");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_Type_Key_Value",
                table: "DNSRecords",
                columns: new[] { "Type", "Key", "Value" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DNSRecords_Type_Key_Value",
                table: "DNSRecords");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_Type_Key",
                table: "DNSRecords",
                columns: new[] { "Type", "Key" },
                unique: true);
        }
    }
}
