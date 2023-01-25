using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class pwnctl_schema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ZoneDepth = table.Column<int>(type: "integer", nullable: false),
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
                name: "NetworkRanges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstAddress = table.Column<string>(type: "text", nullable: true),
                    NetPrefixBits = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkRanges", x => x.Id);
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
                    Topic = table.Column<int>(type: "integer", nullable: false),
                    CheckOutOfScope = table.Column<bool>(type: "boolean", nullable: false)
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
                    Filter = table.Column<string>(type: "text", nullable: true),
                    MatchOutOfScope = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "Sockets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    TransportProtocol = table.Column<int>(type: "integer", nullable: false),
                    NetworkHostId = table.Column<string>(type: "text", nullable: true),
                    DomainNameId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sockets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sockets_Domains_DomainNameId",
                        column: x => x.DomainNameId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sockets_Hosts_NetworkHostId",
                        column: x => x.NetworkHostId,
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
                name: "HttpEndpoints",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    SocketAddressId = table.Column<string>(type: "text", nullable: true),
                    ParentEndpointId = table.Column<string>(type: "text", nullable: true),
                    Scheme = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpEndpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HttpEndpoints_HttpEndpoints_ParentEndpointId",
                        column: x => x.ParentEndpointId,
                        principalTable: "HttpEndpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HttpEndpoints_Sockets_SocketAddressId",
                        column: x => x.SocketAddressId,
                        principalTable: "Sockets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HttpHosts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ServiceId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpHosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HttpHosts_Sockets_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Sockets",
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
                name: "HttpParameters",
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
                    table.PrimaryKey("PK_HttpParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HttpParameters_HttpEndpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "HttpEndpoints",
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
                    SubjectClass_Class = table.Column<string>(type: "text", nullable: true),
                    NetworkHostId = table.Column<string>(type: "text", nullable: true),
                    NetworkSocketId = table.Column<string>(type: "text", nullable: true),
                    HttpEndpointId = table.Column<string>(type: "text", nullable: true),
                    DomainNameId = table.Column<string>(type: "text", nullable: true),
                    DomainNameRecordId = table.Column<string>(type: "text", nullable: true),
                    NetworkRangeId = table.Column<string>(type: "text", nullable: true),
                    EmailId = table.Column<string>(type: "text", nullable: true),
                    HttpParameterId = table.Column<string>(type: "text", nullable: true),
                    HttpHostId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetRecords_DNSRecords_DomainNameRecordId",
                        column: x => x.DomainNameRecordId,
                        principalTable: "DNSRecords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRecords_Domains_DomainNameId",
                        column: x => x.DomainNameId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRecords_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRecords_Hosts_NetworkHostId",
                        column: x => x.NetworkHostId,
                        principalTable: "Hosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRecords_HttpEndpoints_HttpEndpointId",
                        column: x => x.HttpEndpointId,
                        principalTable: "HttpEndpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRecords_HttpHosts_HttpHostId",
                        column: x => x.HttpHostId,
                        principalTable: "HttpHosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRecords_HttpParameters_HttpParameterId",
                        column: x => x.HttpParameterId,
                        principalTable: "HttpParameters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRecords_NetworkRanges_NetworkRangeId",
                        column: x => x.NetworkRangeId,
                        principalTable: "NetworkRanges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRecords_Programs_OwningProgramId",
                        column: x => x.OwningProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetRecords_Sockets_NetworkSocketId",
                        column: x => x.NetworkSocketId,
                        principalTable: "Sockets",
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
                name: "IX_AssetRecords_DomainNameId",
                table: "AssetRecords",
                column: "DomainNameId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_DomainNameRecordId",
                table: "AssetRecords",
                column: "DomainNameRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_EmailId",
                table: "AssetRecords",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_HttpEndpointId",
                table: "AssetRecords",
                column: "HttpEndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_HttpHostId",
                table: "AssetRecords",
                column: "HttpHostId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_HttpParameterId",
                table: "AssetRecords",
                column: "HttpParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_NetworkHostId",
                table: "AssetRecords",
                column: "NetworkHostId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_NetworkRangeId",
                table: "AssetRecords",
                column: "NetworkRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_NetworkSocketId",
                table: "AssetRecords",
                column: "NetworkSocketId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_OwningProgramId",
                table: "AssetRecords",
                column: "OwningProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_DomainId",
                table: "DNSRecords",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_HostId",
                table: "DNSRecords",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_Type_Key_Value",
                table: "DNSRecords",
                columns: new[] { "Type", "Key", "Value" },
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
                name: "IX_Hosts_IP",
                table: "Hosts",
                column: "IP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HttpEndpoints_ParentEndpointId",
                table: "HttpEndpoints",
                column: "ParentEndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_HttpEndpoints_SocketAddressId",
                table: "HttpEndpoints",
                column: "SocketAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_HttpEndpoints_Url",
                table: "HttpEndpoints",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HttpHosts_ServiceId",
                table: "HttpHosts",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_HttpParameters_EndpointId",
                table: "HttpParameters",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_HttpParameters_Url_Name_Type",
                table: "HttpParameters",
                columns: new[] { "Url", "Name", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetworkRanges_FirstAddress_NetPrefixBits",
                table: "NetworkRanges",
                columns: new[] { "FirstAddress", "NetPrefixBits" },
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
                name: "IX_Sockets_Address",
                table: "Sockets",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sockets_DomainNameId",
                table: "Sockets",
                column: "DomainNameId");

            migrationBuilder.CreateIndex(
                name: "IX_Sockets_NetworkHostId",
                table: "Sockets",
                column: "NetworkHostId");

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
                name: "DNSRecords");

            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "HttpHosts");

            migrationBuilder.DropTable(
                name: "HttpParameters");

            migrationBuilder.DropTable(
                name: "NetworkRanges");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "HttpEndpoints");

            migrationBuilder.DropTable(
                name: "OperationalPolicies");

            migrationBuilder.DropTable(
                name: "Sockets");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "Hosts");
        }
    }
}
