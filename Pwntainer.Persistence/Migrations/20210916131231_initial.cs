using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pwntainer.Persistence.Migrations
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
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    IP = table.Column<string>(type: "TEXT", nullable: true),
                    OperatingSystem_Family = table.Column<string>(type: "TEXT", nullable: true),
                    OperatingSystem_Version = table.Column<string>(type: "TEXT", nullable: true),
                    OperatingSystem_Build = table.Column<string>(type: "TEXT", nullable: true),
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    CIDR = table.Column<string>(type: "TEXT", nullable: true),
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransportProtocol = table.Column<int>(type: "INTEGER", nullable: false),
                    Port = table.Column<ushort>(type: "INTEGER", nullable: false),
                    IP = table.Column<string>(type: "TEXT", nullable: true),
                    Protocol = table.Column<string>(type: "TEXT", nullable: true),
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServiceTagId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tag_Subject = table.Column<int>(type: "INTEGER", nullable: true),
                    Tag_Type = table.Column<string>(type: "TEXT", nullable: true),
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WildcardDomains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Pattern = table.Column<string>(type: "TEXT", nullable: true),
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WildcardDomains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ARecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    DomainId = table.Column<int>(type: "INTEGER", nullable: false),
                    HostId = table.Column<int>(type: "INTEGER", nullable: false),
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ARecords_Domains_Id",
                        column: x => x.Id,
                        principalTable: "Domains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ARecords_Hosts_Id",
                        column: x => x.Id,
                        principalTable: "Hosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Endpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Uri = table.Column<string>(type: "TEXT", nullable: true),
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                name: "EndpointTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    EndpointId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tag_Subject = table.Column<int>(type: "INTEGER", nullable: true),
                    Tag_Type = table.Column<string>(type: "TEXT", nullable: true),
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndpointTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndpointTags_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Endpoints_ServiceId",
                table: "Endpoints",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_EndpointTags_EndpointId",
                table: "EndpointTags",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualHosts_ServiceId",
                table: "VirtualHosts",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ARecords");

            migrationBuilder.DropTable(
                name: "EndpointTags");

            migrationBuilder.DropTable(
                name: "NetRanges");

            migrationBuilder.DropTable(
                name: "ServiceTags");

            migrationBuilder.DropTable(
                name: "VirtualHosts");

            migrationBuilder.DropTable(
                name: "WildcardDomains");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "Hosts");

            migrationBuilder.DropTable(
                name: "Endpoints");

            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
