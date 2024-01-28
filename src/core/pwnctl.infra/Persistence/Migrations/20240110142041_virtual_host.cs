using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class virtual_host : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "virtual_hosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SocketAddress = table.Column<string>(type: "text", nullable: false),
                    Hostname = table.Column<string>(type: "text", nullable: false),
                    SocketId = table.Column<Guid>(type: "uuid", nullable: false),
                    DomainId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_virtual_hosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_virtual_hosts_domain_names_DomainId",
                        column: x => x.DomainId,
                        principalTable: "domain_names",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_virtual_hosts_network_sockets_SocketId",
                        column: x => x.SocketId,
                        principalTable: "network_sockets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_virtual_hosts_DomainId",
                table: "virtual_hosts",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_virtual_hosts_SocketAddress_Hostname",
                table: "virtual_hosts",
                columns: new[] { "SocketAddress", "Hostname" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_virtual_hosts_SocketId",
                table: "virtual_hosts",
                column: "SocketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "virtual_hosts");
        }
    }
}
