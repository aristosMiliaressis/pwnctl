using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pwnctl.infra.Persistence.Migrations
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
                    RegistrationDomainId = table.Column<string>(type: "text", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Domains_Domains_RegistrationDomainId",
                        column: x => x.RegistrationDomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Hosts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IP = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
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
                    NetPrefixBits = table.Column<int>(type: "integer", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationProviderSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationProviderSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    Filter = table.Column<string>(type: "text", nullable: true),
                    Topic = table.Column<string>(type: "text", nullable: true),
                    Severity = table.Column<string>(type: "text", nullable: true)
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
                    Name = table.Column<string>(type: "text", nullable: true),
                    Blacklist = table.Column<string>(type: "text", nullable: true),
                    Whitelist = table.Column<string>(type: "text", nullable: true),
                    MaxAggressiveness = table.Column<int>(type: "integer", nullable: true),
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
                    Subject = table.Column<string>(type: "text", nullable: true),
                    Filter = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<string>(type: "text", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
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
                    DomainId = table.Column<string>(type: "text", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
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
                    DomainId = table.Column<string>(type: "text", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
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
                    DomainId = table.Column<string>(type: "text", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "NotificationChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Filter = table.Column<string>(type: "text", nullable: true),
                    ProviderId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationChannels_NotificationProviderSettings_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "NotificationProviderSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Scheme = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endpoints", x => x.Id);
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
                    ServiceId = table.Column<string>(type: "text", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
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
                    Name = table.Column<string>(type: "text", nullable: true),
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
                    UrlEncodedCsValues = table.Column<string>(type: "text", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    HostId = table.Column<string>(type: "text", nullable: true),
                    ServiceId = table.Column<string>(type: "text", nullable: true),
                    EndpointId = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<string>(type: "text", nullable: true),
                    DNSRecordId = table.Column<string>(type: "text", nullable: true),
                    NetRangeId = table.Column<string>(type: "text", nullable: true),
                    EmailId = table.Column<string>(type: "text", nullable: true),
                    KeywordId = table.Column<string>(type: "text", nullable: true),
                    ParameterId = table.Column<string>(type: "text", nullable: true),
                    VirtualHostId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_DNSRecords_DNSRecordId",
                        column: x => x.DNSRecordId,
                        principalTable: "DNSRecords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_NetRanges_NetRangeId",
                        column: x => x.NetRangeId,
                        principalTable: "NetRanges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Parameters_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "Parameters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_VirtualHosts_VirtualHostId",
                        column: x => x.VirtualHostId,
                        principalTable: "VirtualHosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
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
                    EmailId = table.Column<string>(type: "text", nullable: true),
                    ParameterId = table.Column<string>(type: "text", nullable: true),
                    VirtualHostId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
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
                name: "IX_Domains_RegistrationDomainId",
                table: "Domains",
                column: "RegistrationDomainId");

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
                column: "DomainId");

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
                name: "IX_NotificationChannels_ProviderId",
                table: "NotificationChannels",
                column: "ProviderId");

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
                name: "IX_Tags_EmailId",
                table: "Tags",
                column: "EmailId");

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
                name: "IX_Tags_KeywordId",
                table: "Tags",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NetRangeId_Name",
                table: "Tags",
                columns: new[] { "NetRangeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ParameterId",
                table: "Tags",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ServiceId_Name",
                table: "Tags",
                columns: new[] { "ServiceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_VirtualHostId",
                table: "Tags",
                column: "VirtualHostId");

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

            migrationBuilder.CreateIndex(
                name: "IX_VirtualHosts_ServiceId",
                table: "VirtualHosts",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationChannels");

            migrationBuilder.DropTable(
                name: "NotificationRules");

            migrationBuilder.DropTable(
                name: "ScopeDefinitions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "NotificationProviderSettings");

            migrationBuilder.DropTable(
                name: "Programs");

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
                name: "TaskDefinitions");

            migrationBuilder.DropTable(
                name: "VirtualHosts");

            migrationBuilder.DropTable(
                name: "OperationalPolicies");

            migrationBuilder.DropTable(
                name: "Endpoints");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "Hosts");
        }
    }
}
