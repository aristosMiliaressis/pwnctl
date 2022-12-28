using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IsRegistrationDomain = table.Column<bool>(type: "boolean", nullable: false),
                    ParentDomainId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Domains_Domains_ParentDomainId",
                        column: x => x.ParentDomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Hosts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IP = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NetRanges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstAddress = table.Column<string>(type: "text", nullable: true),
                    NetPrefixBits = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    SubjectClass_Class = table.Column<string>(type: "text", nullable: true),
                    Filter = table.Column<string>(type: "text", nullable: true),
                    Topic = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationalPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Blacklist = table.Column<string>(type: "text", nullable: true),
                    Whitelist = table.Column<string>(type: "text", nullable: true),
                    MaxAggressiveness = table.Column<long>(type: "bigint", nullable: true),
                    AllowActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationalPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    CommandTemplate = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Aggressiveness = table.Column<int>(type: "integer", nullable: false),
                    SubjectClass_Class = table.Column<string>(type: "text", nullable: true),
                    Filter = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CloudService",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Hostname = table.Column<string>(type: "text", nullable: true),
                    Service = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudService_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emails_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Word = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Keywords_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DNSRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    HostId = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DNSRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DNSRecords_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DNSRecords_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: true),
                    TransportProtocol = table.Column<int>(type: "integer", nullable: false),
                    ApplicationProtocol = table.Column<string>(type: "text", nullable: true),
                    HostId = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Services_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Platform = table.Column<string>(type: "text", nullable: true),
                    PolicyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_OperationalPolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "OperationalPolicies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Endpoints",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    ServiceId = table.Column<string>(type: "text", nullable: true),
                    ParentEndpointId = table.Column<string>(type: "text", nullable: true),
                    Scheme = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Endpoints_Endpoints_ParentEndpointId",
                        column: x => x.ParentEndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Endpoints_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VirtualHosts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ServiceId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualHosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualHosts_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScopeDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Pattern = table.Column<string>(type: "text", nullable: true),
                    ProgramId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScopeDefinitions_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EndpointId = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UrlEncodedCsValues = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameters_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false),
                    OwningProgramId = table.Column<int>(type: "integer", nullable: true),
                    SubjectClass_Class = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetRecords_CloudService_Id",
                        column: x => x.Id,
                        principalTable: "CloudService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_DNSRecords_Id",
                        column: x => x.Id,
                        principalTable: "DNSRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_Domains_Id",
                        column: x => x.Id,
                        principalTable: "Domains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_Emails_Id",
                        column: x => x.Id,
                        principalTable: "Emails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_Endpoints_Id",
                        column: x => x.Id,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_Hosts_Id",
                        column: x => x.Id,
                        principalTable: "Hosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_Keywords_Id",
                        column: x => x.Id,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_NetRanges_Id",
                        column: x => x.Id,
                        principalTable: "NetRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_Parameters_Id",
                        column: x => x.Id,
                        principalTable: "Parameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_Programs_OwningProgramId",
                        column: x => x.OwningProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRecords_Services_Id",
                        column: x => x.Id,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetRecords_VirtualHosts_Id",
                        column: x => x.Id,
                        principalTable: "VirtualHosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    RecordId = table.Column<string>(type: "text", nullable: true),
                    AssetRecordId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_AssetRecords_AssetRecordId",
                        column: x => x.AssetRecordId,
                        principalTable: "AssetRecords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_AssetRecords_RecordId",
                        column: x => x.RecordId,
                        principalTable: "AssetRecords",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DefinitionId = table.Column<int>(type: "integer", nullable: false),
                    ExitCode = table.Column<int>(type: "integer", nullable: true),
                    QueuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    RecordId = table.Column<string>(type: "text", nullable: true),
                    AssetRecordId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskEntries_AssetRecords_AssetRecordId",
                        column: x => x.AssetRecordId,
                        principalTable: "AssetRecords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskEntries_AssetRecords_RecordId",
                        column: x => x.RecordId,
                        principalTable: "AssetRecords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskEntries_TaskDefinitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "TaskDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_OwningProgramId",
                table: "AssetRecords",
                column: "OwningProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudService_DomainId",
                table: "CloudService",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudService_Hostname",
                table: "CloudService",
                column: "Hostname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_DomainId",
                table: "DNSRecords",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_HostId",
                table: "DNSRecords",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_Type_Key",
                table: "DNSRecords",
                columns: new[] { "Type", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Domains_Name",
                table: "Domains",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Domains_ParentDomainId",
                table: "Domains",
                column: "ParentDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_Address",
                table: "Emails",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emails_DomainId",
                table: "Emails",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_ParentEndpointId",
                table: "Endpoints",
                column: "ParentEndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_ServiceId",
                table: "Endpoints",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_Url",
                table: "Endpoints",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hosts_IP",
                table: "Hosts",
                column: "IP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_DomainId",
                table: "Keywords",
                column: "DomainId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_Word",
                table: "Keywords",
                column: "Word",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetRanges_FirstAddress_NetPrefixBits",
                table: "NetRanges",
                columns: new[] { "FirstAddress", "NetPrefixBits" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_EndpointId",
                table: "Parameters",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_Url_Name_Type",
                table: "Parameters",
                columns: new[] { "Url", "Name", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Programs_PolicyId",
                table: "Programs",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScopeDefinitions_ProgramId",
                table: "ScopeDefinitions",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_DomainId",
                table: "Services",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_HostId",
                table: "Services",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_Origin",
                table: "Services",
                column: "Origin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AssetRecordId",
                table: "Tags",
                column: "AssetRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_RecordId_Name",
                table: "Tags",
                columns: new[] { "RecordId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskEntries_AssetRecordId",
                table: "TaskEntries",
                column: "AssetRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEntries_DefinitionId",
                table: "TaskEntries",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEntries_RecordId",
                table: "TaskEntries",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualHosts_ServiceId",
                table: "VirtualHosts",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationRules");

            migrationBuilder.DropTable(
                name: "ScopeDefinitions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "TaskEntries");

            migrationBuilder.DropTable(
                name: "AssetRecords");

            migrationBuilder.DropTable(
                name: "TaskDefinitions");

            migrationBuilder.DropTable(
                name: "CloudService");

            migrationBuilder.DropTable(
                name: "DNSRecords");

            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "NetRanges");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "VirtualHosts");

            migrationBuilder.DropTable(
                name: "Endpoints");

            migrationBuilder.DropTable(
                name: "OperationalPolicies");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "Hosts");
        }
    }
}
