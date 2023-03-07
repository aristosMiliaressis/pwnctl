using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class httphost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SocketAddress",
                table: "HttpHosts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HttpHosts_Name_SocketAddress",
                table: "HttpHosts",
                columns: new[] { "Name", "SocketAddress" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HttpHosts_Name_SocketAddress",
                table: "HttpHosts");

            migrationBuilder.DropColumn(
                name: "SocketAddress",
                table: "HttpHosts");
        }
    }
}
