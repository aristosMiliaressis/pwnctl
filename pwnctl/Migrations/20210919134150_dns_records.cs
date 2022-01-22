using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace pwnctl.Migrations
{
    public partial class dns_records : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ARecords");

            migrationBuilder.CreateTable(
                name: "DNSRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    HostId = table.Column<int>(type: "INTEGER", nullable: true),
                    DomainId = table.Column<int>(type: "INTEGER", nullable: true),
                    TasksRun = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DNSRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DNSRecords_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DNSRecords_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_DomainId",
                table: "DNSRecords",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_DNSRecords_HostId",
                table: "DNSRecords",
                column: "HostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DNSRecords");

            migrationBuilder.CreateTable(
                name: "ARecords",
                columns: table => new
                {
                    DomainId = table.Column<int>(type: "INTEGER", nullable: false),
                    HostId = table.Column<int>(type: "INTEGER", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARecords", x => new { x.DomainId, x.HostId });
                    table.ForeignKey(
                        name: "FK_ARecords_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ARecords_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ARecords_HostId",
                table: "ARecords",
                column: "HostId");
        }
    }
}
