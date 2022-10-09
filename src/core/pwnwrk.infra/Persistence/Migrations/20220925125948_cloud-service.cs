using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnwrk.infra.Persistence.Migrations
{
    public partial class cloudservice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CloudServiceId",
                table: "Tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CloudServiceId",
                table: "Tags",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CloudService",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Domainname = table.Column<string>(type: "text", nullable: true),
                    Service = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<string>(type: "text", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FoundBy = table.Column<string>(type: "text", nullable: true),
                    InScope = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudService_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CloudServiceId",
                table: "Tasks",
                column: "CloudServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CloudServiceId",
                table: "Tags",
                column: "CloudServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudService_DomainId",
                table: "CloudService",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudService_Domainname",
                table: "CloudService",
                column: "Domainname",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_CloudService_CloudServiceId",
                table: "Tags",
                column: "CloudServiceId",
                principalTable: "CloudService",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_CloudService_CloudServiceId",
                table: "Tasks",
                column: "CloudServiceId",
                principalTable: "CloudService",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_CloudService_CloudServiceId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_CloudService_CloudServiceId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "CloudService");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CloudServiceId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CloudServiceId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CloudServiceId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CloudServiceId",
                table: "Tags");
        }
    }
}
