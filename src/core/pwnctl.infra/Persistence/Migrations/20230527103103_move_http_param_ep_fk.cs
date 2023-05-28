using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class move_http_param_ep_fk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HttpEndpointId",
                table: "http_parameters",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_http_parameters_HttpEndpointId",
                table: "http_parameters",
                column: "HttpEndpointId");

            migrationBuilder.AddForeignKey(
                name: "FK_http_parameters_http_endpoints_HttpEndpointId",
                table: "http_parameters",
                column: "HttpEndpointId",
                principalTable: "http_endpoints",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_http_parameters_http_endpoints_HttpEndpointId",
                table: "http_parameters");

            migrationBuilder.DropIndex(
                name: "IX_http_parameters_HttpEndpointId",
                table: "http_parameters");

            migrationBuilder.DropColumn(
                name: "HttpEndpointId",
                table: "http_parameters");
        }
    }
}
