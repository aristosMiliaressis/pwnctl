using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class pwnctl_lowercase_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_DNSRecords_DomainNameRecordId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Domains_DomainNameId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Emails_EmailId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Hosts_NetworkHostId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_HttpEndpoints_HttpEndpointId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_HttpHosts_HttpHostId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_HttpParameters_HttpParameterId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_NetworkRanges_NetworkRangeId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Programs_OwningProgramId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Sockets_NetworkSocketId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_TaskEntries_FoundByTaskId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_DNSRecords_Domains_DomainId",
                table: "DNSRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_DNSRecords_Hosts_HostId",
                table: "DNSRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Domains_Domains_ParentDomainId",
                table: "Domains");

            migrationBuilder.DropForeignKey(
                name: "FK_Emails_Domains_DomainId",
                table: "Emails");

            migrationBuilder.DropForeignKey(
                name: "FK_HttpEndpoints_HttpEndpoints_ParentEndpointId",
                table: "HttpEndpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_HttpEndpoints_Sockets_SocketAddressId",
                table: "HttpEndpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_HttpHosts_Sockets_ServiceId",
                table: "HttpHosts");

            migrationBuilder.DropForeignKey(
                name: "FK_HttpParameters_HttpEndpoints_EndpointId",
                table: "HttpParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_Programs_OperationalPolicies_PolicyId",
                table: "Programs");

            migrationBuilder.DropForeignKey(
                name: "FK_ScopeDefinitions_Programs_ProgramId",
                table: "ScopeDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sockets_Domains_DomainNameId",
                table: "Sockets");

            migrationBuilder.DropForeignKey(
                name: "FK_Sockets_Hosts_NetworkHostId",
                table: "Sockets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AssetRecords_AssetRecordId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AssetRecords_RecordId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskEntries_AssetRecords_AssetRecordId",
                table: "TaskEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskEntries_AssetRecords_RecordId",
                table: "TaskEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskEntries_TaskDefinitions_DefinitionId",
                table: "TaskEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Programs",
                table: "Programs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Emails",
                table: "Emails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskEntries",
                table: "TaskEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskDefinitions",
                table: "TaskDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sockets",
                table: "Sockets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScopeDefinitions",
                table: "ScopeDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationalPolicies",
                table: "OperationalPolicies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationRules",
                table: "NotificationRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NetworkRanges",
                table: "NetworkRanges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HttpParameters",
                table: "HttpParameters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HttpHosts",
                table: "HttpHosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HttpEndpoints",
                table: "HttpEndpoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hosts",
                table: "Hosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Domains",
                table: "Domains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DNSRecords",
                table: "DNSRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetRecords",
                table: "AssetRecords");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "tags");

            migrationBuilder.RenameTable(
                name: "Programs",
                newName: "programs");

            migrationBuilder.RenameTable(
                name: "Emails",
                newName: "emails");

            migrationBuilder.RenameTable(
                name: "TaskEntries",
                newName: "task_entries");

            migrationBuilder.RenameTable(
                name: "TaskDefinitions",
                newName: "task_definitions");

            migrationBuilder.RenameTable(
                name: "Sockets",
                newName: "network_sockets");

            migrationBuilder.RenameTable(
                name: "ScopeDefinitions",
                newName: "scope_definitions");

            migrationBuilder.RenameTable(
                name: "OperationalPolicies",
                newName: "operation_policies");

            migrationBuilder.RenameTable(
                name: "NotificationRules",
                newName: "notification_rules");

            migrationBuilder.RenameTable(
                name: "NetworkRanges",
                newName: "network_ranges");

            migrationBuilder.RenameTable(
                name: "HttpParameters",
                newName: "http_parameters");

            migrationBuilder.RenameTable(
                name: "HttpHosts",
                newName: "http_hosts");

            migrationBuilder.RenameTable(
                name: "HttpEndpoints",
                newName: "http_endpoints");

            migrationBuilder.RenameTable(
                name: "Hosts",
                newName: "network_hosts");

            migrationBuilder.RenameTable(
                name: "Domains",
                newName: "domain_names");

            migrationBuilder.RenameTable(
                name: "DNSRecords",
                newName: "domain_name_records");

            migrationBuilder.RenameTable(
                name: "AssetRecords",
                newName: "asset_records");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_RecordId_Name",
                table: "tags",
                newName: "IX_tags_RecordId_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_AssetRecordId",
                table: "tags",
                newName: "IX_tags_AssetRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_Programs_PolicyId",
                table: "programs",
                newName: "IX_programs_PolicyId");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_DomainId",
                table: "emails",
                newName: "IX_emails_DomainId");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_Address",
                table: "emails",
                newName: "IX_emails_Address");

            migrationBuilder.RenameIndex(
                name: "IX_TaskEntries_RecordId",
                table: "task_entries",
                newName: "IX_task_entries_RecordId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskEntries_DefinitionId",
                table: "task_entries",
                newName: "IX_task_entries_DefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskEntries_AssetRecordId",
                table: "task_entries",
                newName: "IX_task_entries_AssetRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_Sockets_NetworkHostId",
                table: "network_sockets",
                newName: "IX_network_sockets_NetworkHostId");

            migrationBuilder.RenameIndex(
                name: "IX_Sockets_DomainNameId",
                table: "network_sockets",
                newName: "IX_network_sockets_DomainNameId");

            migrationBuilder.RenameIndex(
                name: "IX_Sockets_Address",
                table: "network_sockets",
                newName: "IX_network_sockets_Address");

            migrationBuilder.RenameIndex(
                name: "IX_ScopeDefinitions_ProgramId",
                table: "scope_definitions",
                newName: "IX_scope_definitions_ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_NetworkRanges_FirstAddress_NetPrefixBits",
                table: "network_ranges",
                newName: "IX_network_ranges_FirstAddress_NetPrefixBits");

            migrationBuilder.RenameIndex(
                name: "IX_HttpParameters_Url_Name_Type",
                table: "http_parameters",
                newName: "IX_http_parameters_Url_Name_Type");

            migrationBuilder.RenameIndex(
                name: "IX_HttpParameters_EndpointId",
                table: "http_parameters",
                newName: "IX_http_parameters_EndpointId");

            migrationBuilder.RenameIndex(
                name: "IX_HttpHosts_ServiceId",
                table: "http_hosts",
                newName: "IX_http_hosts_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_HttpEndpoints_Url",
                table: "http_endpoints",
                newName: "IX_http_endpoints_Url");

            migrationBuilder.RenameIndex(
                name: "IX_HttpEndpoints_SocketAddressId",
                table: "http_endpoints",
                newName: "IX_http_endpoints_SocketAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_HttpEndpoints_ParentEndpointId",
                table: "http_endpoints",
                newName: "IX_http_endpoints_ParentEndpointId");

            migrationBuilder.RenameIndex(
                name: "IX_Hosts_IP",
                table: "network_hosts",
                newName: "IX_network_hosts_IP");

            migrationBuilder.RenameIndex(
                name: "IX_Domains_ParentDomainId",
                table: "domain_names",
                newName: "IX_domain_names_ParentDomainId");

            migrationBuilder.RenameIndex(
                name: "IX_Domains_Name",
                table: "domain_names",
                newName: "IX_domain_names_Name");

            migrationBuilder.RenameIndex(
                name: "IX_DNSRecords_Type_Key_Value",
                table: "domain_name_records",
                newName: "IX_domain_name_records_Type_Key_Value");

            migrationBuilder.RenameIndex(
                name: "IX_DNSRecords_HostId",
                table: "domain_name_records",
                newName: "IX_domain_name_records_HostId");

            migrationBuilder.RenameIndex(
                name: "IX_DNSRecords_DomainId",
                table: "domain_name_records",
                newName: "IX_domain_name_records_DomainId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_OwningProgramId",
                table: "asset_records",
                newName: "IX_asset_records_OwningProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_NetworkSocketId",
                table: "asset_records",
                newName: "IX_asset_records_NetworkSocketId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_NetworkRangeId",
                table: "asset_records",
                newName: "IX_asset_records_NetworkRangeId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_NetworkHostId",
                table: "asset_records",
                newName: "IX_asset_records_NetworkHostId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_HttpParameterId",
                table: "asset_records",
                newName: "IX_asset_records_HttpParameterId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_HttpHostId",
                table: "asset_records",
                newName: "IX_asset_records_HttpHostId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_HttpEndpointId",
                table: "asset_records",
                newName: "IX_asset_records_HttpEndpointId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_FoundByTaskId",
                table: "asset_records",
                newName: "IX_asset_records_FoundByTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_EmailId",
                table: "asset_records",
                newName: "IX_asset_records_EmailId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_DomainNameRecordId",
                table: "asset_records",
                newName: "IX_asset_records_DomainNameRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_DomainNameId",
                table: "asset_records",
                newName: "IX_asset_records_DomainNameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tags",
                table: "tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_programs",
                table: "programs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_emails",
                table: "emails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_task_entries",
                table: "task_entries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_task_definitions",
                table: "task_definitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_network_sockets",
                table: "network_sockets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_scope_definitions",
                table: "scope_definitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_operation_policies",
                table: "operation_policies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_notification_rules",
                table: "notification_rules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_network_ranges",
                table: "network_ranges",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_http_parameters",
                table: "http_parameters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_http_hosts",
                table: "http_hosts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_http_endpoints",
                table: "http_endpoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_network_hosts",
                table: "network_hosts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_domain_names",
                table: "domain_names",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_domain_name_records",
                table: "domain_name_records",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_asset_records",
                table: "asset_records",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_domain_name_records_DomainNameRecordId",
                table: "asset_records",
                column: "DomainNameRecordId",
                principalTable: "domain_name_records",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_domain_names_DomainNameId",
                table: "asset_records",
                column: "DomainNameId",
                principalTable: "domain_names",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_emails_EmailId",
                table: "asset_records",
                column: "EmailId",
                principalTable: "emails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_http_endpoints_HttpEndpointId",
                table: "asset_records",
                column: "HttpEndpointId",
                principalTable: "http_endpoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_http_hosts_HttpHostId",
                table: "asset_records",
                column: "HttpHostId",
                principalTable: "http_hosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_http_parameters_HttpParameterId",
                table: "asset_records",
                column: "HttpParameterId",
                principalTable: "http_parameters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_network_hosts_NetworkHostId",
                table: "asset_records",
                column: "NetworkHostId",
                principalTable: "network_hosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_network_ranges_NetworkRangeId",
                table: "asset_records",
                column: "NetworkRangeId",
                principalTable: "network_ranges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_network_sockets_NetworkSocketId",
                table: "asset_records",
                column: "NetworkSocketId",
                principalTable: "network_sockets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_programs_OwningProgramId",
                table: "asset_records",
                column: "OwningProgramId",
                principalTable: "programs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_task_entries_FoundByTaskId",
                table: "asset_records",
                column: "FoundByTaskId",
                principalTable: "task_entries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_domain_name_records_domain_names_DomainId",
                table: "domain_name_records",
                column: "DomainId",
                principalTable: "domain_names",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_domain_name_records_network_hosts_HostId",
                table: "domain_name_records",
                column: "HostId",
                principalTable: "network_hosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_domain_names_domain_names_ParentDomainId",
                table: "domain_names",
                column: "ParentDomainId",
                principalTable: "domain_names",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_emails_domain_names_DomainId",
                table: "emails",
                column: "DomainId",
                principalTable: "domain_names",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_http_endpoints_http_endpoints_ParentEndpointId",
                table: "http_endpoints",
                column: "ParentEndpointId",
                principalTable: "http_endpoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_http_endpoints_network_sockets_SocketAddressId",
                table: "http_endpoints",
                column: "SocketAddressId",
                principalTable: "network_sockets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_http_hosts_network_sockets_ServiceId",
                table: "http_hosts",
                column: "ServiceId",
                principalTable: "network_sockets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_http_parameters_http_endpoints_EndpointId",
                table: "http_parameters",
                column: "EndpointId",
                principalTable: "http_endpoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_network_sockets_domain_names_DomainNameId",
                table: "network_sockets",
                column: "DomainNameId",
                principalTable: "domain_names",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_network_sockets_network_hosts_NetworkHostId",
                table: "network_sockets",
                column: "NetworkHostId",
                principalTable: "network_hosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_programs_operation_policies_PolicyId",
                table: "programs",
                column: "PolicyId",
                principalTable: "operation_policies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_scope_definitions_programs_ProgramId",
                table: "scope_definitions",
                column: "ProgramId",
                principalTable: "programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tags_asset_records_AssetRecordId",
                table: "tags",
                column: "AssetRecordId",
                principalTable: "asset_records",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tags_asset_records_RecordId",
                table: "tags",
                column: "RecordId",
                principalTable: "asset_records",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_task_entries_asset_records_AssetRecordId",
                table: "task_entries",
                column: "AssetRecordId",
                principalTable: "asset_records",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_task_entries_asset_records_RecordId",
                table: "task_entries",
                column: "RecordId",
                principalTable: "asset_records",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_task_entries_task_definitions_DefinitionId",
                table: "task_entries",
                column: "DefinitionId",
                principalTable: "task_definitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
                name: "FK_asset_records_emails_EmailId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_http_endpoints_HttpEndpointId",
                table: "asset_records");

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
                name: "FK_asset_records_network_ranges_NetworkRangeId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_network_sockets_NetworkSocketId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_programs_OwningProgramId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_task_entries_FoundByTaskId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_domain_name_records_domain_names_DomainId",
                table: "domain_name_records");

            migrationBuilder.DropForeignKey(
                name: "FK_domain_name_records_network_hosts_HostId",
                table: "domain_name_records");

            migrationBuilder.DropForeignKey(
                name: "FK_domain_names_domain_names_ParentDomainId",
                table: "domain_names");

            migrationBuilder.DropForeignKey(
                name: "FK_emails_domain_names_DomainId",
                table: "emails");

            migrationBuilder.DropForeignKey(
                name: "FK_http_endpoints_http_endpoints_ParentEndpointId",
                table: "http_endpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_http_endpoints_network_sockets_SocketAddressId",
                table: "http_endpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_http_hosts_network_sockets_ServiceId",
                table: "http_hosts");

            migrationBuilder.DropForeignKey(
                name: "FK_http_parameters_http_endpoints_EndpointId",
                table: "http_parameters");

            migrationBuilder.DropForeignKey(
                name: "FK_network_sockets_domain_names_DomainNameId",
                table: "network_sockets");

            migrationBuilder.DropForeignKey(
                name: "FK_network_sockets_network_hosts_NetworkHostId",
                table: "network_sockets");

            migrationBuilder.DropForeignKey(
                name: "FK_programs_operation_policies_PolicyId",
                table: "programs");

            migrationBuilder.DropForeignKey(
                name: "FK_scope_definitions_programs_ProgramId",
                table: "scope_definitions");

            migrationBuilder.DropForeignKey(
                name: "FK_tags_asset_records_AssetRecordId",
                table: "tags");

            migrationBuilder.DropForeignKey(
                name: "FK_tags_asset_records_RecordId",
                table: "tags");

            migrationBuilder.DropForeignKey(
                name: "FK_task_entries_asset_records_AssetRecordId",
                table: "task_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_task_entries_asset_records_RecordId",
                table: "task_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_task_entries_task_definitions_DefinitionId",
                table: "task_entries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tags",
                table: "tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_programs",
                table: "programs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_emails",
                table: "emails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_task_entries",
                table: "task_entries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_task_definitions",
                table: "task_definitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_scope_definitions",
                table: "scope_definitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_operation_policies",
                table: "operation_policies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_notification_rules",
                table: "notification_rules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_network_sockets",
                table: "network_sockets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_network_ranges",
                table: "network_ranges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_network_hosts",
                table: "network_hosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_http_parameters",
                table: "http_parameters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_http_hosts",
                table: "http_hosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_http_endpoints",
                table: "http_endpoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_domain_names",
                table: "domain_names");

            migrationBuilder.DropPrimaryKey(
                name: "PK_domain_name_records",
                table: "domain_name_records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_asset_records",
                table: "asset_records");

            migrationBuilder.RenameTable(
                name: "tags",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "programs",
                newName: "Programs");

            migrationBuilder.RenameTable(
                name: "emails",
                newName: "Emails");

            migrationBuilder.RenameTable(
                name: "task_entries",
                newName: "TaskEntries");

            migrationBuilder.RenameTable(
                name: "task_definitions",
                newName: "TaskDefinitions");

            migrationBuilder.RenameTable(
                name: "scope_definitions",
                newName: "ScopeDefinitions");

            migrationBuilder.RenameTable(
                name: "operation_policies",
                newName: "OperationalPolicies");

            migrationBuilder.RenameTable(
                name: "notification_rules",
                newName: "NotificationRules");

            migrationBuilder.RenameTable(
                name: "network_sockets",
                newName: "Sockets");

            migrationBuilder.RenameTable(
                name: "network_ranges",
                newName: "NetworkRanges");

            migrationBuilder.RenameTable(
                name: "network_hosts",
                newName: "Hosts");

            migrationBuilder.RenameTable(
                name: "http_parameters",
                newName: "HttpParameters");

            migrationBuilder.RenameTable(
                name: "http_hosts",
                newName: "HttpHosts");

            migrationBuilder.RenameTable(
                name: "http_endpoints",
                newName: "HttpEndpoints");

            migrationBuilder.RenameTable(
                name: "domain_names",
                newName: "Domains");

            migrationBuilder.RenameTable(
                name: "domain_name_records",
                newName: "DNSRecords");

            migrationBuilder.RenameTable(
                name: "asset_records",
                newName: "AssetRecords");

            migrationBuilder.RenameIndex(
                name: "IX_tags_RecordId_Name",
                table: "Tags",
                newName: "IX_Tags_RecordId_Name");

            migrationBuilder.RenameIndex(
                name: "IX_tags_AssetRecordId",
                table: "Tags",
                newName: "IX_Tags_AssetRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_programs_PolicyId",
                table: "Programs",
                newName: "IX_Programs_PolicyId");

            migrationBuilder.RenameIndex(
                name: "IX_emails_DomainId",
                table: "Emails",
                newName: "IX_Emails_DomainId");

            migrationBuilder.RenameIndex(
                name: "IX_emails_Address",
                table: "Emails",
                newName: "IX_Emails_Address");

            migrationBuilder.RenameIndex(
                name: "IX_task_entries_RecordId",
                table: "TaskEntries",
                newName: "IX_TaskEntries_RecordId");

            migrationBuilder.RenameIndex(
                name: "IX_task_entries_DefinitionId",
                table: "TaskEntries",
                newName: "IX_TaskEntries_DefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_task_entries_AssetRecordId",
                table: "TaskEntries",
                newName: "IX_TaskEntries_AssetRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_scope_definitions_ProgramId",
                table: "ScopeDefinitions",
                newName: "IX_ScopeDefinitions_ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_network_sockets_NetworkHostId",
                table: "Sockets",
                newName: "IX_Sockets_NetworkHostId");

            migrationBuilder.RenameIndex(
                name: "IX_network_sockets_DomainNameId",
                table: "Sockets",
                newName: "IX_Sockets_DomainNameId");

            migrationBuilder.RenameIndex(
                name: "IX_network_sockets_Address",
                table: "Sockets",
                newName: "IX_Sockets_Address");

            migrationBuilder.RenameIndex(
                name: "IX_network_ranges_FirstAddress_NetPrefixBits",
                table: "NetworkRanges",
                newName: "IX_NetworkRanges_FirstAddress_NetPrefixBits");

            migrationBuilder.RenameIndex(
                name: "IX_network_hosts_IP",
                table: "Hosts",
                newName: "IX_Hosts_IP");

            migrationBuilder.RenameIndex(
                name: "IX_http_parameters_Url_Name_Type",
                table: "HttpParameters",
                newName: "IX_HttpParameters_Url_Name_Type");

            migrationBuilder.RenameIndex(
                name: "IX_http_parameters_EndpointId",
                table: "HttpParameters",
                newName: "IX_HttpParameters_EndpointId");

            migrationBuilder.RenameIndex(
                name: "IX_http_hosts_ServiceId",
                table: "HttpHosts",
                newName: "IX_HttpHosts_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_http_endpoints_Url",
                table: "HttpEndpoints",
                newName: "IX_HttpEndpoints_Url");

            migrationBuilder.RenameIndex(
                name: "IX_http_endpoints_SocketAddressId",
                table: "HttpEndpoints",
                newName: "IX_HttpEndpoints_SocketAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_http_endpoints_ParentEndpointId",
                table: "HttpEndpoints",
                newName: "IX_HttpEndpoints_ParentEndpointId");

            migrationBuilder.RenameIndex(
                name: "IX_domain_names_ParentDomainId",
                table: "Domains",
                newName: "IX_Domains_ParentDomainId");

            migrationBuilder.RenameIndex(
                name: "IX_domain_names_Name",
                table: "Domains",
                newName: "IX_Domains_Name");

            migrationBuilder.RenameIndex(
                name: "IX_domain_name_records_Type_Key_Value",
                table: "DNSRecords",
                newName: "IX_DNSRecords_Type_Key_Value");

            migrationBuilder.RenameIndex(
                name: "IX_domain_name_records_HostId",
                table: "DNSRecords",
                newName: "IX_DNSRecords_HostId");

            migrationBuilder.RenameIndex(
                name: "IX_domain_name_records_DomainId",
                table: "DNSRecords",
                newName: "IX_DNSRecords_DomainId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_OwningProgramId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_OwningProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_NetworkSocketId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_NetworkSocketId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_NetworkRangeId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_NetworkRangeId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_NetworkHostId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_NetworkHostId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_HttpParameterId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_HttpParameterId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_HttpHostId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_HttpHostId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_HttpEndpointId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_HttpEndpointId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_FoundByTaskId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_FoundByTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_EmailId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_EmailId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_DomainNameRecordId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_DomainNameRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_asset_records_DomainNameId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_DomainNameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Programs",
                table: "Programs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emails",
                table: "Emails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskEntries",
                table: "TaskEntries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskDefinitions",
                table: "TaskDefinitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScopeDefinitions",
                table: "ScopeDefinitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationalPolicies",
                table: "OperationalPolicies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationRules",
                table: "NotificationRules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sockets",
                table: "Sockets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NetworkRanges",
                table: "NetworkRanges",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hosts",
                table: "Hosts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HttpParameters",
                table: "HttpParameters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HttpHosts",
                table: "HttpHosts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HttpEndpoints",
                table: "HttpEndpoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Domains",
                table: "Domains",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DNSRecords",
                table: "DNSRecords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetRecords",
                table: "AssetRecords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_DNSRecords_DomainNameRecordId",
                table: "AssetRecords",
                column: "DomainNameRecordId",
                principalTable: "DNSRecords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Domains_DomainNameId",
                table: "AssetRecords",
                column: "DomainNameId",
                principalTable: "Domains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Emails_EmailId",
                table: "AssetRecords",
                column: "EmailId",
                principalTable: "Emails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Hosts_NetworkHostId",
                table: "AssetRecords",
                column: "NetworkHostId",
                principalTable: "Hosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_HttpEndpoints_HttpEndpointId",
                table: "AssetRecords",
                column: "HttpEndpointId",
                principalTable: "HttpEndpoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_HttpHosts_HttpHostId",
                table: "AssetRecords",
                column: "HttpHostId",
                principalTable: "HttpHosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_HttpParameters_HttpParameterId",
                table: "AssetRecords",
                column: "HttpParameterId",
                principalTable: "HttpParameters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_NetworkRanges_NetworkRangeId",
                table: "AssetRecords",
                column: "NetworkRangeId",
                principalTable: "NetworkRanges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Programs_OwningProgramId",
                table: "AssetRecords",
                column: "OwningProgramId",
                principalTable: "Programs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Sockets_NetworkSocketId",
                table: "AssetRecords",
                column: "NetworkSocketId",
                principalTable: "Sockets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_TaskEntries_FoundByTaskId",
                table: "AssetRecords",
                column: "FoundByTaskId",
                principalTable: "TaskEntries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DNSRecords_Domains_DomainId",
                table: "DNSRecords",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DNSRecords_Hosts_HostId",
                table: "DNSRecords",
                column: "HostId",
                principalTable: "Hosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Domains_Domains_ParentDomainId",
                table: "Domains",
                column: "ParentDomainId",
                principalTable: "Domains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_Domains_DomainId",
                table: "Emails",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HttpEndpoints_HttpEndpoints_ParentEndpointId",
                table: "HttpEndpoints",
                column: "ParentEndpointId",
                principalTable: "HttpEndpoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HttpEndpoints_Sockets_SocketAddressId",
                table: "HttpEndpoints",
                column: "SocketAddressId",
                principalTable: "Sockets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HttpHosts_Sockets_ServiceId",
                table: "HttpHosts",
                column: "ServiceId",
                principalTable: "Sockets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HttpParameters_HttpEndpoints_EndpointId",
                table: "HttpParameters",
                column: "EndpointId",
                principalTable: "HttpEndpoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_OperationalPolicies_PolicyId",
                table: "Programs",
                column: "PolicyId",
                principalTable: "OperationalPolicies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScopeDefinitions_Programs_ProgramId",
                table: "ScopeDefinitions",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sockets_Domains_DomainNameId",
                table: "Sockets",
                column: "DomainNameId",
                principalTable: "Domains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sockets_Hosts_NetworkHostId",
                table: "Sockets",
                column: "NetworkHostId",
                principalTable: "Hosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AssetRecords_AssetRecordId",
                table: "Tags",
                column: "AssetRecordId",
                principalTable: "AssetRecords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AssetRecords_RecordId",
                table: "Tags",
                column: "RecordId",
                principalTable: "AssetRecords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskEntries_AssetRecords_AssetRecordId",
                table: "TaskEntries",
                column: "AssetRecordId",
                principalTable: "AssetRecords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskEntries_AssetRecords_RecordId",
                table: "TaskEntries",
                column: "RecordId",
                principalTable: "AssetRecords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskEntries_TaskDefinitions_DefinitionId",
                table: "TaskEntries",
                column: "DefinitionId",
                principalTable: "TaskDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
