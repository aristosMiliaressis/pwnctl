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
                name: "domain_names",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ZoneDepth = table.Column<int>(type: "integer", nullable: false),
                    ParentDomainId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domain_names", x => x.Id);
                    table.ForeignKey(
                        name: "FK_domain_names_domain_names_ParentDomainId",
                        column: x => x.ParentDomainId,
                        principalTable: "domain_names",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "network_hosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IP = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_network_hosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "network_ranges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstAddress = table.Column<string>(type: "text", nullable: true),
                    NetPrefixBits = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_network_ranges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notification_rules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName_Value = table.Column<string>(type: "text", nullable: true),
                    SubjectClass_Value = table.Column<string>(type: "text", nullable: true),
                    Topic = table.Column<int>(type: "integer", nullable: false),
                    Filter = table.Column<string>(type: "text", nullable: true),
                    Template = table.Column<string>(type: "text", nullable: true),
                    CheckOutOfScope = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "scope_aggregates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName_Value = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scope_aggregates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "scope_definitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Pattern = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scope_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "task_profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName_Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_emails_domain_names_DomainId",
                        column: x => x.DomainId,
                        principalTable: "domain_names",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "domain_name_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    HostId = table.Column<Guid>(type: "uuid", nullable: true),
                    DomainId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domain_name_records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_domain_name_records_domain_names_DomainId",
                        column: x => x.DomainId,
                        principalTable: "domain_names",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_domain_name_records_network_hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "network_hosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "network_sockets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    TransportProtocol = table.Column<int>(type: "integer", nullable: false),
                    NetworkHostId = table.Column<Guid>(type: "uuid", nullable: true),
                    DomainNameId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_network_sockets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_network_sockets_domain_names_DomainNameId",
                        column: x => x.DomainNameId,
                        principalTable: "domain_names",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_network_sockets_network_hosts_NetworkHostId",
                        column: x => x.NetworkHostId,
                        principalTable: "network_hosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "scope_definition_aggregates",
                columns: table => new
                {
                    AggregateId = table.Column<int>(type: "integer", nullable: false),
                    DefinitionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scope_definition_aggregates", x => new { x.AggregateId, x.DefinitionId });
                    table.ForeignKey(
                        name: "FK_scope_definition_aggregates_scope_aggregates_AggregateId",
                        column: x => x.AggregateId,
                        principalTable: "scope_aggregates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scope_definition_aggregates_scope_definitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "scope_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "policies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Blacklist = table.Column<string>(type: "text", nullable: true),
                    Whitelist = table.Column<string>(type: "text", nullable: true),
                    MaxAggressiveness = table.Column<long>(type: "bigint", nullable: true),
                    OnlyPassive = table.Column<bool>(type: "boolean", nullable: false),
                    TaskProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_policies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_policies_task_profiles_TaskProfileId",
                        column: x => x.TaskProfileId,
                        principalTable: "task_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "task_definitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName_Value = table.Column<string>(type: "text", nullable: true),
                    CommandTemplate = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Aggressiveness = table.Column<int>(type: "integer", nullable: false),
                    SubjectClass_Value = table.Column<string>(type: "text", nullable: true),
                    Filter = table.Column<string>(type: "text", nullable: true),
                    MatchOutOfScope = table.Column<bool>(type: "boolean", nullable: false),
                    ProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_definitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_task_definitions_task_profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "task_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "http_endpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    SocketAddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentEndpointId = table.Column<Guid>(type: "uuid", nullable: true),
                    Scheme = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_http_endpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_http_endpoints_http_endpoints_ParentEndpointId",
                        column: x => x.ParentEndpointId,
                        principalTable: "http_endpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_http_endpoints_network_sockets_SocketAddressId",
                        column: x => x.SocketAddressId,
                        principalTable: "network_sockets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "http_hosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SocketAddress = table.Column<string>(type: "text", nullable: true),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_http_hosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_http_hosts_network_sockets_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "network_sockets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "operations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName_Value = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    PolicyId = table.Column<int>(type: "integer", nullable: true),
                    ScopeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_operations_policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "policies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_operations_scope_aggregates_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "scope_aggregates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "http_parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EndpointId = table.Column<Guid>(type: "uuid", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UrlEncodedCsValues = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_http_parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_http_parameters_http_endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "http_endpoints",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "asset_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundByTaskId = table.Column<int>(type: "integer", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false),
                    ScopeId = table.Column<int>(type: "integer", nullable: true),
                    SubjectClass_Value = table.Column<string>(type: "text", nullable: true),
                    NetworkHostId = table.Column<Guid>(type: "uuid", nullable: true),
                    NetworkSocketId = table.Column<Guid>(type: "uuid", nullable: true),
                    HttpEndpointId = table.Column<Guid>(type: "uuid", nullable: true),
                    DomainNameId = table.Column<Guid>(type: "uuid", nullable: true),
                    DomainNameRecordId = table.Column<Guid>(type: "uuid", nullable: true),
                    NetworkRangeId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmailId = table.Column<Guid>(type: "uuid", nullable: true),
                    HttpParameterId = table.Column<Guid>(type: "uuid", nullable: true),
                    HttpHostId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset_records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_asset_records_domain_name_records_DomainNameRecordId",
                        column: x => x.DomainNameRecordId,
                        principalTable: "domain_name_records",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_asset_records_domain_names_DomainNameId",
                        column: x => x.DomainNameId,
                        principalTable: "domain_names",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_asset_records_emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "emails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_asset_records_http_endpoints_HttpEndpointId",
                        column: x => x.HttpEndpointId,
                        principalTable: "http_endpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_asset_records_http_hosts_HttpHostId",
                        column: x => x.HttpHostId,
                        principalTable: "http_hosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_asset_records_http_parameters_HttpParameterId",
                        column: x => x.HttpParameterId,
                        principalTable: "http_parameters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_asset_records_network_hosts_NetworkHostId",
                        column: x => x.NetworkHostId,
                        principalTable: "network_hosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_asset_records_network_ranges_NetworkRangeId",
                        column: x => x.NetworkRangeId,
                        principalTable: "network_ranges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_asset_records_network_sockets_NetworkSocketId",
                        column: x => x.NetworkSocketId,
                        principalTable: "network_sockets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_asset_records_scope_definitions_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "scope_definitions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleId = table.Column<int>(type: "integer", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notifications_asset_records_RecordId",
                        column: x => x.RecordId,
                        principalTable: "asset_records",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notifications_notification_rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "notification_rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tags_asset_records_RecordId",
                        column: x => x.RecordId,
                        principalTable: "asset_records",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "task_entries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExitCode = table.Column<int>(type: "integer", nullable: true),
                    QueuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    OperationId = table.Column<int>(type: "integer", nullable: false),
                    DefinitionId = table.Column<int>(type: "integer", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_task_entries_asset_records_RecordId",
                        column: x => x.RecordId,
                        principalTable: "asset_records",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_task_entries_operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_task_entries_task_definitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "task_definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_DomainNameId",
                table: "asset_records",
                column: "DomainNameId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_DomainNameRecordId",
                table: "asset_records",
                column: "DomainNameRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_EmailId",
                table: "asset_records",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_FoundByTaskId",
                table: "asset_records",
                column: "FoundByTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_HttpEndpointId",
                table: "asset_records",
                column: "HttpEndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_HttpHostId",
                table: "asset_records",
                column: "HttpHostId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_HttpParameterId",
                table: "asset_records",
                column: "HttpParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_NetworkHostId",
                table: "asset_records",
                column: "NetworkHostId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_NetworkRangeId",
                table: "asset_records",
                column: "NetworkRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_NetworkSocketId",
                table: "asset_records",
                column: "NetworkSocketId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_ScopeId",
                table: "asset_records",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_domain_name_records_DomainId",
                table: "domain_name_records",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_domain_name_records_HostId",
                table: "domain_name_records",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_domain_name_records_Type_Key_Value",
                table: "domain_name_records",
                columns: new[] { "Type", "Key", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_domain_names_Name",
                table: "domain_names",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_domain_names_ParentDomainId",
                table: "domain_names",
                column: "ParentDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_emails_Address",
                table: "emails",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_emails_DomainId",
                table: "emails",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_http_endpoints_ParentEndpointId",
                table: "http_endpoints",
                column: "ParentEndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_http_endpoints_SocketAddressId",
                table: "http_endpoints",
                column: "SocketAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_http_endpoints_Url",
                table: "http_endpoints",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_http_hosts_Name_SocketAddress",
                table: "http_hosts",
                columns: new[] { "Name", "SocketAddress" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_http_hosts_ServiceId",
                table: "http_hosts",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_http_parameters_EndpointId",
                table: "http_parameters",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_http_parameters_Url_Name_Type",
                table: "http_parameters",
                columns: new[] { "Url", "Name", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_network_hosts_IP",
                table: "network_hosts",
                column: "IP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_network_ranges_FirstAddress_NetPrefixBits",
                table: "network_ranges",
                columns: new[] { "FirstAddress", "NetPrefixBits" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_network_sockets_Address",
                table: "network_sockets",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_network_sockets_DomainNameId",
                table: "network_sockets",
                column: "DomainNameId");

            migrationBuilder.CreateIndex(
                name: "IX_network_sockets_NetworkHostId",
                table: "network_sockets",
                column: "NetworkHostId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_RecordId",
                table: "notifications",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_RuleId",
                table: "notifications",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_operations_PolicyId",
                table: "operations",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_operations_ScopeId",
                table: "operations",
                column: "ScopeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_policies_TaskProfileId",
                table: "policies",
                column: "TaskProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_scope_definition_aggregates_DefinitionId",
                table: "scope_definition_aggregates",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_tags_RecordId_Name",
                table: "tags",
                columns: new[] { "RecordId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_task_definitions_ProfileId",
                table: "task_definitions",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_task_entries_DefinitionId",
                table: "task_entries",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_task_entries_OperationId",
                table: "task_entries",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_task_entries_RecordId",
                table: "task_entries",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_task_entries_FoundByTaskId",
                table: "asset_records",
                column: "FoundByTaskId",
                principalTable: "task_entries",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_domain_name_records_DomainNameRecordId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_domain_names_DomainNameId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_emails_domain_names_DomainId",
                table: "emails");

            migrationBuilder.DropForeignKey(
                name: "FK_network_sockets_domain_names_DomainNameId",
                table: "network_sockets");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_emails_EmailId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_http_endpoints_HttpEndpointId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_http_parameters_http_endpoints_EndpointId",
                table: "http_parameters");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_http_hosts_HttpHostId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_http_parameters_HttpParameterId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_network_hosts_NetworkHostId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_network_sockets_network_hosts_NetworkHostId",
                table: "network_sockets");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_network_ranges_NetworkRangeId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_network_sockets_NetworkSocketId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_scope_definitions_ScopeId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_task_entries_FoundByTaskId",
                table: "asset_records");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "scope_definition_aggregates");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "notification_rules");

            migrationBuilder.DropTable(
                name: "domain_name_records");

            migrationBuilder.DropTable(
                name: "domain_names");

            migrationBuilder.DropTable(
                name: "emails");

            migrationBuilder.DropTable(
                name: "http_endpoints");

            migrationBuilder.DropTable(
                name: "http_hosts");

            migrationBuilder.DropTable(
                name: "http_parameters");

            migrationBuilder.DropTable(
                name: "network_hosts");

            migrationBuilder.DropTable(
                name: "network_ranges");

            migrationBuilder.DropTable(
                name: "network_sockets");

            migrationBuilder.DropTable(
                name: "scope_definitions");

            migrationBuilder.DropTable(
                name: "task_entries");

            migrationBuilder.DropTable(
                name: "asset_records");

            migrationBuilder.DropTable(
                name: "operations");

            migrationBuilder.DropTable(
                name: "task_definitions");

            migrationBuilder.DropTable(
                name: "policies");

            migrationBuilder.DropTable(
                name: "scope_aggregates");

            migrationBuilder.DropTable(
                name: "task_profiles");
        }
    }
}
