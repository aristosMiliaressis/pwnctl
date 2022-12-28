using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_CloudService_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_DNSRecords_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Domains_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Emails_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Endpoints_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Hosts_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Keywords_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_NetRanges_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Parameters_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Services_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_VirtualHosts_Id",
                table: "AssetRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_CloudService_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "CloudService",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_DNSRecords_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "DNSRecords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Domains_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Domains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Emails_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Emails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Endpoints_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Endpoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Hosts_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Hosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Keywords_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Keywords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_NetRanges_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "NetRanges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Parameters_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Parameters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Services_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_VirtualHosts_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "VirtualHosts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_CloudService_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_DNSRecords_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Domains_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Emails_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Endpoints_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Hosts_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Keywords_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_NetRanges_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Parameters_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Services_Id",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_VirtualHosts_Id",
                table: "AssetRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_CloudService_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "CloudService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_DNSRecords_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "DNSRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Domains_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Emails_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Endpoints_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Endpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Hosts_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Hosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Keywords_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Keywords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_NetRanges_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "NetRanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Parameters_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Services_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_VirtualHosts_Id",
                table: "AssetRecords",
                column: "Id",
                principalTable: "VirtualHosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
