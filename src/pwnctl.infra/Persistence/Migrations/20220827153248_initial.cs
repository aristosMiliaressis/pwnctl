using Microsoft.EntityFrameworkCore.Migrations;

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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    IsRegistrationDomain = table.Column<bool>(type: "INTEGER", nullable: false),
                    RegistrationDomainId = table.Column<int>(type: "INTEGER", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IP = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NetRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstAddress = table.Column<string>(type: "TEXT", nullable: true),
                    NetPrefixBits = table.Column<ushort>(type: "INTEGER", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationalPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Blacklist = table.Column<string>(type: "TEXT", nullable: true),
                    Whitelist = table.Column<string>(type: "TEXT", nullable: true),
                    MaxAggressiveness = table.Column<int>(type: "INTEGER", nullable: true),
                    AllowActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationalPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShortName = table.Column<string>(type: "TEXT", nullable: true),
                    CommandTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Aggressiveness = table.Column<int>(type: "INTEGER", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    Filter = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DNSRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    HostId = table.Column<int>(type: "INTEGER", nullable: true),
                    DomainId = table.Column<int>(type: "INTEGER", nullable: true),
                    DomainId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    HostId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
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
                        name: "FK_DNSRecords_Domains_DomainId1",
                        column: x => x.DomainId1,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DNSRecords_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DNSRecords_Hosts_HostId1",
                        column: x => x.HostId1,
                        principalTable: "Hosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Port = table.Column<ushort>(type: "INTEGER", nullable: false),
                    Origin = table.Column<string>(type: "TEXT", nullable: true),
                    TransportProtocol = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationProtocol = table.Column<string>(type: "TEXT", nullable: true),
                    HostId = table.Column<int>(type: "INTEGER", nullable: true),
                    DomainId = table.Column<int>(type: "INTEGER", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Platform = table.Column<string>(type: "TEXT", nullable: true),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Uri = table.Column<string>(type: "TEXT", nullable: true),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Scheme = table.Column<string>(type: "TEXT", nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Endpoints_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VirtualHosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualHosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualHosts_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScopeDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Pattern = table.Column<string>(type: "TEXT", nullable: true),
                    ProgramId = table.Column<int>(type: "INTEGER", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EndpointId = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    UrlEncodedCsValues = table.Column<string>(type: "TEXT", nullable: true),
                    RequestId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameters_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EndpointId = table.Column<int>(type: "INTEGER", nullable: false),
                    ResponseId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResponseId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    Method = table.Column<string>(type: "TEXT", nullable: true),
                    Body = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Responses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    BodyHash = table.Column<string>(type: "TEXT", nullable: true),
                    TimeMs = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Responses_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    HostId = table.Column<int>(type: "INTEGER", nullable: true),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: true),
                    EndpointId = table.Column<int>(type: "INTEGER", nullable: true),
                    DomainId = table.Column<int>(type: "INTEGER", nullable: true),
                    DNSRecordId = table.Column<int>(type: "INTEGER", nullable: true),
                    NetRangeId = table.Column<int>(type: "INTEGER", nullable: true),
                    DNSRecordId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    DomainId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    EndpointId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    HostId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    NetRangeId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    ParameterId = table.Column<int>(type: "INTEGER", nullable: true),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResponseId = table.Column<int>(type: "INTEGER", nullable: true),
                    ServiceId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    VirtualHostId = table.Column<int>(type: "INTEGER", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tag_DNSRecords_DNSRecordId",
                        column: x => x.DNSRecordId,
                        principalTable: "DNSRecords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_DNSRecords_DNSRecordId1",
                        column: x => x.DNSRecordId1,
                        principalTable: "DNSRecords",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Domains_DomainId1",
                        column: x => x.DomainId1,
                        principalTable: "Domains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Endpoints_EndpointId1",
                        column: x => x.EndpointId1,
                        principalTable: "Endpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Hosts_HostId1",
                        column: x => x.HostId1,
                        principalTable: "Hosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_NetRanges_NetRangeId",
                        column: x => x.NetRangeId,
                        principalTable: "NetRanges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_NetRanges_NetRangeId1",
                        column: x => x.NetRangeId1,
                        principalTable: "NetRanges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Parameters_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "Parameters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Responses_ResponseId",
                        column: x => x.ResponseId,
                        principalTable: "Responses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_Services_ServiceId1",
                        column: x => x.ServiceId1,
                        principalTable: "Services",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tag_VirtualHosts_VirtualHostId",
                        column: x => x.VirtualHostId,
                        principalTable: "VirtualHosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DefinitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReturnCode = table.Column<int>(type: "INTEGER", nullable: true),
                    QueuedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Arguments = table.Column<string>(type: "TEXT", nullable: true),
                    HostId = table.Column<int>(type: "INTEGER", nullable: true),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: true),
                    EndpointId = table.Column<int>(type: "INTEGER", nullable: true),
                    DomainId = table.Column<int>(type: "INTEGER", nullable: true),
                    DNSRecordId = table.Column<int>(type: "INTEGER", nullable: true),
                    NetRangeId = table.Column<int>(type: "INTEGER", nullable: true),
                    ParameterId = table.Column<int>(type: "INTEGER", nullable: true),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResponseId = table.Column<int>(type: "INTEGER", nullable: true),
                    VirtualHostId = table.Column<int>(type: "INTEGER", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                        name: "FK_Tasks_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Responses_ResponseId",
                        column: x => x.ResponseId,
                        principalTable: "Responses",
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
                name: "IX_DNSRecords_DomainId1",
                table: "DNSRecords",
                column: "DomainId1");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_HostId",
                table: "DNSRecords",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_HostId1",
                table: "DNSRecords",
                column: "HostId1");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_RegistrationDomainId",
                table: "Domains",
                column: "RegistrationDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_ServiceId",
                table: "Endpoints",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_EndpointId",
                table: "Parameters",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_RequestId",
                table: "Parameters",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_RequestId1",
                table: "Parameters",
                column: "RequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_PolicyId",
                table: "Programs",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_EndpointId",
                table: "Requests",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ResponseId1",
                table: "Requests",
                column: "ResponseId1");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_RequestId",
                table: "Responses",
                column: "RequestId");

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
                name: "IX_Tag_DNSRecordId",
                table: "Tag",
                column: "DNSRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_DNSRecordId1",
                table: "Tag",
                column: "DNSRecordId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_DomainId",
                table: "Tag",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_DomainId1",
                table: "Tag",
                column: "DomainId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_EndpointId",
                table: "Tag",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_EndpointId1",
                table: "Tag",
                column: "EndpointId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_HostId",
                table: "Tag",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_HostId1",
                table: "Tag",
                column: "HostId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_NetRangeId",
                table: "Tag",
                column: "NetRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_NetRangeId1",
                table: "Tag",
                column: "NetRangeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_ParameterId",
                table: "Tag",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_RequestId",
                table: "Tag",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_ResponseId",
                table: "Tag",
                column: "ResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_ServiceId",
                table: "Tag",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_ServiceId1",
                table: "Tag",
                column: "ServiceId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_VirtualHostId",
                table: "Tag",
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
                name: "IX_Tasks_EndpointId",
                table: "Tasks",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_HostId",
                table: "Tasks",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_NetRangeId",
                table: "Tasks",
                column: "NetRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ParameterId",
                table: "Tasks",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_RequestId",
                table: "Tasks",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ResponseId",
                table: "Tasks",
                column: "ResponseId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Parameters_Requests_RequestId",
                table: "Parameters",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parameters_Requests_RequestId1",
                table: "Parameters",
                column: "RequestId1",
                principalTable: "Requests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Responses_ResponseId1",
                table: "Requests",
                column: "ResponseId1",
                principalTable: "Responses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Domains_DomainId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Hosts_HostId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Endpoints_Services_ServiceId",
                table: "Endpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Endpoints_EndpointId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Requests_RequestId",
                table: "Responses");

            migrationBuilder.DropTable(
                name: "ScopeDefinitions");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "DNSRecords");

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
                name: "Domains");

            migrationBuilder.DropTable(
                name: "Hosts");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Endpoints");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Responses");
        }
    }
}
