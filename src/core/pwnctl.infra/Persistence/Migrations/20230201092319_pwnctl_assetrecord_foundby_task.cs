using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class pwnctl_assetrecord_foundby_task : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoundBy",
                table: "AssetRecords");

            migrationBuilder.AddColumn<int>(
                name: "FoundByTaskId",
                table: "AssetRecords",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetRecords_FoundByTaskId",
                table: "AssetRecords",
                column: "FoundByTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_TaskEntries_FoundByTaskId",
                table: "AssetRecords",
                column: "FoundByTaskId",
                principalTable: "TaskEntries",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_TaskEntries_FoundByTaskId",
                table: "AssetRecords");

            migrationBuilder.DropIndex(
                name: "IX_AssetRecords_FoundByTaskId",
                table: "AssetRecords");

            migrationBuilder.DropColumn(
                name: "FoundByTaskId",
                table: "AssetRecords");

            migrationBuilder.AddColumn<string>(
                name: "FoundBy",
                table: "AssetRecords",
                type: "text",
                nullable: true);
        }
    }
}
