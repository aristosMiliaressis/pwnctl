using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Persistence.Migrations
{
    public partial class uniqness : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_DNSRecordId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_DomainId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_EndpointId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_HostId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_NetRangeId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ServiceId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Parameters_EndpointId",
                table: "Parameters");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DNSRecordId_Name",
                table: "Tags",
                columns: new[] { "DNSRecordId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DomainId_Name",
                table: "Tags",
                columns: new[] { "DomainId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_EndpointId_Name",
                table: "Tags",
                columns: new[] { "EndpointId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_HostId_Name",
                table: "Tags",
                columns: new[] { "HostId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NetRangeId_Name",
                table: "Tags",
                columns: new[] { "NetRangeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ServiceId_Name",
                table: "Tags",
                columns: new[] { "ServiceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_Origin",
                table: "Services",
                column: "Origin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_EndpointId_Name_Type",
                table: "Parameters",
                columns: new[] { "EndpointId", "Name", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetRanges_FirstAddress_NetPrefixBits",
                table: "NetRanges",
                columns: new[] { "FirstAddress", "NetPrefixBits" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hosts_IP",
                table: "Hosts",
                column: "IP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_Uri",
                table: "Endpoints",
                column: "Uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Domains_Name",
                table: "Domains",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_Type_Key",
                table: "DNSRecords",
                columns: new[] { "Type", "Key" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_DNSRecordId_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_DomainId_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_EndpointId_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_HostId_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_NetRangeId_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ServiceId_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Services_Origin",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Parameters_EndpointId_Name_Type",
                table: "Parameters");

            migrationBuilder.DropIndex(
                name: "IX_NetRanges_FirstAddress_NetPrefixBits",
                table: "NetRanges");

            migrationBuilder.DropIndex(
                name: "IX_Hosts_IP",
                table: "Hosts");

            migrationBuilder.DropIndex(
                name: "IX_Endpoints_Uri",
                table: "Endpoints");

            migrationBuilder.DropIndex(
                name: "IX_Domains_Name",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_DNSRecords_Type_Key",
                table: "DNSRecords");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DNSRecordId",
                table: "Tags",
                column: "DNSRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DomainId",
                table: "Tags",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_EndpointId",
                table: "Tags",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_HostId",
                table: "Tags",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NetRangeId",
                table: "Tags",
                column: "NetRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ServiceId",
                table: "Tags",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_EndpointId",
                table: "Parameters",
                column: "EndpointId");
        }
    }
}
