using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pwnwrk.infra.Persistence.Migrations
{
    public partial class refac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ScopeDefinitions");

            migrationBuilder.RenameColumn(
                name: "Domainname",
                table: "CloudService",
                newName: "Hostname");

            migrationBuilder.RenameIndex(
                name: "IX_CloudService_Domainname",
                table: "CloudService",
                newName: "IX_CloudService_Hostname");

            migrationBuilder.CreateTable(
                name: "TaskRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DefinitionId = table.Column<int>(type: "integer", nullable: false),
                    ReturnCode = table.Column<int>(type: "integer", nullable: true),
                    QueuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Arguments = table.Column<string>(type: "text", nullable: true),
                    HostId = table.Column<string>(type: "text", nullable: true),
                    ServiceId = table.Column<string>(type: "text", nullable: true),
                    EndpointId = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<string>(type: "text", nullable: true),
                    DNSRecordId = table.Column<string>(type: "text", nullable: true),
                    NetRangeId = table.Column<string>(type: "text", nullable: true),
                    KeywordId = table.Column<string>(type: "text", nullable: true),
                    CloudServiceId = table.Column<string>(type: "text", nullable: true),
                    EmailId = table.Column<string>(type: "text", nullable: true),
                    ParameterId = table.Column<string>(type: "text", nullable: true),
                    VirtualHostId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskRecords_CloudService_CloudServiceId",
                        column: x => x.CloudServiceId,
                        principalTable: "CloudService",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskRecords_DNSRecords_DNSRecordId",
                        column: x => x.DNSRecordId,
                        principalTable: "DNSRecords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskRecords_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskRecords_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskRecords_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskRecords_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskRecords_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskRecords_NetRanges_NetRangeId",
                        column: x => x.NetRangeId,
                        principalTable: "NetRanges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskRecords_Parameters_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "Parameters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskRecords_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskRecords_TaskDefinitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "TaskDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskRecords_VirtualHosts_VirtualHostId",
                        column: x => x.VirtualHostId,
                        principalTable: "VirtualHosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_CloudServiceId",
                table: "TaskRecords",
                column: "CloudServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_DefinitionId",
                table: "TaskRecords",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_DNSRecordId",
                table: "TaskRecords",
                column: "DNSRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_DomainId",
                table: "TaskRecords",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_EmailId",
                table: "TaskRecords",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_EndpointId",
                table: "TaskRecords",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_HostId",
                table: "TaskRecords",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_KeywordId",
                table: "TaskRecords",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_NetRangeId",
                table: "TaskRecords",
                column: "NetRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_ParameterId",
                table: "TaskRecords",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_ServiceId",
                table: "TaskRecords",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRecords_VirtualHostId",
                table: "TaskRecords",
                column: "VirtualHostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskRecords");

            migrationBuilder.RenameColumn(
                name: "Hostname",
                table: "CloudService",
                newName: "Domainname");

            migrationBuilder.RenameIndex(
                name: "IX_CloudService_Hostname",
                table: "CloudService",
                newName: "IX_CloudService_Domainname");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ScopeDefinitions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CloudServiceId = table.Column<string>(type: "text", nullable: true),
                    DefinitionId = table.Column<int>(type: "integer", nullable: false),
                    DNSRecordId = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<string>(type: "text", nullable: true),
                    EndpointId = table.Column<string>(type: "text", nullable: true),
                    HostId = table.Column<string>(type: "text", nullable: true),
                    KeywordId = table.Column<string>(type: "text", nullable: true),
                    NetRangeId = table.Column<string>(type: "text", nullable: true),
                    ServiceId = table.Column<string>(type: "text", nullable: true),
                    Arguments = table.Column<string>(type: "text", nullable: true),
                    EmailId = table.Column<string>(type: "text", nullable: true),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ParameterId = table.Column<string>(type: "text", nullable: true),
                    QueuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReturnCode = table.Column<int>(type: "integer", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VirtualHostId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_CloudService_CloudServiceId",
                        column: x => x.CloudServiceId,
                        principalTable: "CloudService",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_DNSRecords_DNSRecordId",
                        column: x => x.DNSRecordId,
                        principalTable: "DNSRecords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_NetRanges_NetRangeId",
                        column: x => x.NetRangeId,
                        principalTable: "NetRanges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Parameters_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "Parameters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_TaskDefinitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "TaskDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_VirtualHosts_VirtualHostId",
                        column: x => x.VirtualHostId,
                        principalTable: "VirtualHosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CloudServiceId",
                table: "Tasks",
                column: "CloudServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DefinitionId",
                table: "Tasks",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DNSRecordId",
                table: "Tasks",
                column: "DNSRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DomainId",
                table: "Tasks",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EmailId",
                table: "Tasks",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EndpointId",
                table: "Tasks",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_HostId",
                table: "Tasks",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_KeywordId",
                table: "Tasks",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_NetRangeId",
                table: "Tasks",
                column: "NetRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ParameterId",
                table: "Tasks",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ServiceId",
                table: "Tasks",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_VirtualHostId",
                table: "Tasks",
                column: "VirtualHostId");
        }
    }
}
