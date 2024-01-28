using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class base_endpoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_http_endpoints_http_endpoints_ParentEndpointId",
                table: "http_endpoints");

            migrationBuilder.RenameColumn(
                name: "ParentEndpointId",
                table: "http_endpoints",
                newName: "BaseEndpointId");

            migrationBuilder.RenameIndex(
                name: "IX_http_endpoints_ParentEndpointId",
                table: "http_endpoints",
                newName: "IX_http_endpoints_BaseEndpointId");

            migrationBuilder.AddForeignKey(
                name: "FK_http_endpoints_http_endpoints_BaseEndpointId",
                table: "http_endpoints",
                column: "BaseEndpointId",
                principalTable: "http_endpoints",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_http_endpoints_http_endpoints_BaseEndpointId",
                table: "http_endpoints");

            migrationBuilder.RenameColumn(
                name: "BaseEndpointId",
                table: "http_endpoints",
                newName: "ParentEndpointId");

            migrationBuilder.RenameIndex(
                name: "IX_http_endpoints_BaseEndpointId",
                table: "http_endpoints",
                newName: "IX_http_endpoints_ParentEndpointId");

            migrationBuilder.AddForeignKey(
                name: "FK_http_endpoints_http_endpoints_ParentEndpointId",
                table: "http_endpoints",
                column: "ParentEndpointId",
                principalTable: "http_endpoints",
                principalColumn: "Id");
        }
    }
}
