using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace pwnctl.Migrations
{
    public partial class scoperegex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WildcardDomains");

            migrationBuilder.CreateTable(
                name: "ScopeRegexes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Pattern = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeRegexes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScopeRegexes");

            migrationBuilder.CreateTable(
                name: "WildcardDomains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FoundAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InScope = table.Column<bool>(type: "INTEGER", nullable: false),
                    Pattern = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WildcardDomains", x => x.Id);
                });
        }
    }
}
