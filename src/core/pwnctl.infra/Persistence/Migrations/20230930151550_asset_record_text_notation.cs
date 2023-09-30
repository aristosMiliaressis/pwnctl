using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class asset_record_text_notation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextNotation",
                table: "asset_records",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_asset_records_TextNotation",
                table: "asset_records",
                column: "TextNotation",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_asset_records_TextNotation",
                table: "asset_records");

            migrationBuilder.DropColumn(
                name: "TextNotation",
                table: "asset_records");
        }
    }
}
