using Microsoft.EntityFrameworkCore.Migrations;

namespace pwnctl.Migrations
{
    public partial class stuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TasksRun",
                table: "WildcardDomains");

            migrationBuilder.DropColumn(
                name: "TasksRun",
                table: "VirtualHosts");

            migrationBuilder.DropColumn(
                name: "TasksRun",
                table: "ServiceTags");

            migrationBuilder.DropColumn(
                name: "IP",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Protocol",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CIDR",
                table: "NetRanges");

            migrationBuilder.DropColumn(
                name: "TasksRun",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "TasksRun",
                table: "EndpointTags");

            migrationBuilder.DropColumn(
                name: "TasksRun",
                table: "Domains");

            migrationBuilder.RenameColumn(
                name: "TasksRun",
                table: "Services",
                newName: "ApplicationProtocol");

            migrationBuilder.RenameColumn(
                name: "TasksRun",
                table: "NetRanges",
                newName: "FirstAddress");

            migrationBuilder.RenameColumn(
                name: "TasksRun",
                table: "Endpoints",
                newName: "Scheme");

            migrationBuilder.RenameColumn(
                name: "TasksRun",
                table: "DNSRecords",
                newName: "Key");

            migrationBuilder.AddColumn<bool>(
                name: "InScope",
                table: "WildcardDomains",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InScope",
                table: "VirtualHosts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InScope",
                table: "ServiceTags",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "Services",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HostId",
                table: "Services",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InScope",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InScope",
                table: "NetRanges",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ushort>(
                name: "NetPrefixBits",
                table: "NetRanges",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<bool>(
                name: "InScope",
                table: "Hosts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Hosts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "InScope",
                table: "EndpointTags",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InScope",
                table: "Endpoints",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Endpoints",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRegistrationDomain",
                table: "Domains",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RegistrationDomainId",
                table: "Domains",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DomainId1",
                table: "DNSRecords",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InScope",
                table: "DNSRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Services_DomainId",
                table: "Services",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_HostId",
                table: "Services",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_RegistrationDomainId",
                table: "Domains",
                column: "RegistrationDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_DomainId1",
                table: "DNSRecords",
                column: "DomainId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DNSRecords_Domains_DomainId1",
                table: "DNSRecords",
                column: "DomainId1",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Domains_Domains_RegistrationDomainId",
                table: "Domains",
                column: "RegistrationDomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Domains_DomainId",
                table: "Services",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Hosts_HostId",
                table: "Services",
                column: "HostId",
                principalTable: "Hosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DNSRecords_Domains_DomainId1",
                table: "DNSRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Domains_Domains_RegistrationDomainId",
                table: "Domains");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Domains_DomainId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Hosts_HostId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_DomainId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_HostId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Domains_RegistrationDomainId",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_DNSRecords_DomainId1",
                table: "DNSRecords");

            migrationBuilder.DropColumn(
                name: "InScope",
                table: "WildcardDomains");

            migrationBuilder.DropColumn(
                name: "InScope",
                table: "VirtualHosts");

            migrationBuilder.DropColumn(
                name: "InScope",
                table: "ServiceTags");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "HostId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "InScope",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "InScope",
                table: "NetRanges");

            migrationBuilder.DropColumn(
                name: "NetPrefixBits",
                table: "NetRanges");

            migrationBuilder.DropColumn(
                name: "InScope",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "InScope",
                table: "EndpointTags");

            migrationBuilder.DropColumn(
                name: "InScope",
                table: "Endpoints");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Endpoints");

            migrationBuilder.DropColumn(
                name: "IsRegistrationDomain",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "RegistrationDomainId",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "DomainId1",
                table: "DNSRecords");

            migrationBuilder.DropColumn(
                name: "InScope",
                table: "DNSRecords");

            migrationBuilder.RenameColumn(
                name: "ApplicationProtocol",
                table: "Services",
                newName: "TasksRun");

            migrationBuilder.RenameColumn(
                name: "FirstAddress",
                table: "NetRanges",
                newName: "TasksRun");

            migrationBuilder.RenameColumn(
                name: "Scheme",
                table: "Endpoints",
                newName: "TasksRun");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "DNSRecords",
                newName: "TasksRun");

            migrationBuilder.AddColumn<string>(
                name: "TasksRun",
                table: "WildcardDomains",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TasksRun",
                table: "VirtualHosts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TasksRun",
                table: "ServiceTags",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IP",
                table: "Services",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Protocol",
                table: "Services",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CIDR",
                table: "NetRanges",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TasksRun",
                table: "Hosts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TasksRun",
                table: "EndpointTags",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TasksRun",
                table: "Domains",
                type: "TEXT",
                nullable: true);
        }
    }
}
