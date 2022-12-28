using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class initial3 : Migration
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

            migrationBuilder.AddColumn<string>(
                name: "CloudServiceId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DNSRecordId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DomainId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndpointId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeywordId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetRangeId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParameterId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VirtualHostId",
                table: "AssetRecords",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_CloudServiceId",
                table: "AssetRecords",
                column: "CloudServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_DNSRecordId",
                table: "AssetRecords",
                column: "DNSRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_DomainId",
                table: "AssetRecords",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_EmailId",
                table: "AssetRecords",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_EndpointId",
                table: "AssetRecords",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_HostId",
                table: "AssetRecords",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_KeywordId",
                table: "AssetRecords",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_NetRangeId",
                table: "AssetRecords",
                column: "NetRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_ParameterId",
                table: "AssetRecords",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_ServiceId",
                table: "AssetRecords",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_VirtualHostId",
                table: "AssetRecords",
                column: "VirtualHostId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_CloudService_CloudServiceId",
                table: "AssetRecords",
                column: "CloudServiceId",
                principalTable: "CloudService",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_DNSRecords_DNSRecordId",
                table: "AssetRecords",
                column: "DNSRecordId",
                principalTable: "DNSRecords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Domains_DomainId",
                table: "AssetRecords",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Emails_EmailId",
                table: "AssetRecords",
                column: "EmailId",
                principalTable: "Emails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Endpoints_EndpointId",
                table: "AssetRecords",
                column: "EndpointId",
                principalTable: "Endpoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Hosts_HostId",
                table: "AssetRecords",
                column: "HostId",
                principalTable: "Hosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Keywords_KeywordId",
                table: "AssetRecords",
                column: "KeywordId",
                principalTable: "Keywords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_NetRanges_NetRangeId",
                table: "AssetRecords",
                column: "NetRangeId",
                principalTable: "NetRanges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Parameters_ParameterId",
                table: "AssetRecords",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Services_ServiceId",
                table: "AssetRecords",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_VirtualHosts_VirtualHostId",
                table: "AssetRecords",
                column: "VirtualHostId",
                principalTable: "VirtualHosts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_CloudService_CloudServiceId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_DNSRecords_DNSRecordId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Domains_DomainId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Emails_EmailId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Endpoints_EndpointId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Hosts_HostId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Keywords_KeywordId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_NetRanges_NetRangeId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Parameters_ParameterId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Services_ServiceId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_VirtualHosts_VirtualHostId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_CloudServiceId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_DNSRecordId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_DomainId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_EmailId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_EndpointId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_HostId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_KeywordId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_NetRangeId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_ParameterId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_ServiceId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_VirtualHostId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "CloudServiceId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "DNSRecordId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "EndpointId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "HostId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "KeywordId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "NetRangeId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "ParameterId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "VirtualHostId",
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
    }
}
