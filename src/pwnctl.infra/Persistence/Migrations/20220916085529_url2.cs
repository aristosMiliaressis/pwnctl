using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Persistence.Migrations
{
    public partial class url2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeywordId",
                table: "Tasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeywordId",
                table: "Tags",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Word = table.Column<string>(type: "TEXT", nullable: true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRoutable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_KeywordId",
                table: "Tasks",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_KeywordId",
                table: "Tags",
                column: "KeywordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Keywords_KeywordId",
                table: "Tags",
                column: "KeywordId",
                principalTable: "Keywords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Keywords_KeywordId",
                table: "Tasks",
                column: "KeywordId",
                principalTable: "Keywords",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Keywords_KeywordId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Keywords_KeywordId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_KeywordId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tags_KeywordId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "KeywordId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "KeywordId",
                table: "Tags");
        }
    }
}
