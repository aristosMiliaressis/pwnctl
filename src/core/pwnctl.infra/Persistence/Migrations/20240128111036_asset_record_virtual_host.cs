using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class asset_record_virtual_host : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VirtualHostId",
                table: "asset_records",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_VirtualHostId",
                table: "asset_records",
                column: "VirtualHostId");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_virtual_hosts_VirtualHostId",
                table: "asset_records",
                column: "VirtualHostId",
                principalTable: "virtual_hosts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_virtual_hosts_VirtualHostId",
                table: "asset_records");

            migrationBuilder.DropIndex(
                name: "IX_asset_records_VirtualHostId",
                table: "asset_records");

            migrationBuilder.DropColumn(
                name: "VirtualHostId",
                table: "asset_records");
        }
    }
}
