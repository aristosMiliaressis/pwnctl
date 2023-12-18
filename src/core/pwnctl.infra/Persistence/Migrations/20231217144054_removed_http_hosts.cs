using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class removed_http_hosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "http_hosts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "http_hosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SocketAddress = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_http_hosts_Name_SocketAddress",
                table: "http_hosts",
                columns: new[] { "Name", "SocketAddress" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_http_hosts_ServiceId",
                table: "http_hosts",
                column: "ServiceId");
        }
    }
}
