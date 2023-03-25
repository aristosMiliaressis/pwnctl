using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class notification_entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AssetRecords_AssetRecordId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskEntries_AssetRecords_AssetRecordId",
                table: "TaskEntries");

            migrationBuilder.DropIndex(
                name: "IX_TaskEntries_AssetRecordId",
                table: "TaskEntries");

            migrationBuilder.DropIndex(
                name: "IX_Tags_AssetRecordId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "AssetRecordId",
                table: "TaskEntries");

            migrationBuilder.DropColumn(
                name: "AssetRecordId",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleId = table.Column<int>(type: "integer", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AssetRecords_RecordId",
                        column: x => x.RecordId,
                        principalTable: "AssetRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_NotificationRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "NotificationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecordId",
                table: "Notifications",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RuleId",
                table: "Notifications",
                column: "RuleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.AddColumn<Guid>(
                name: "AssetRecordId",
                table: "TaskEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetRecordId",
                table: "Tags",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskEntries_AssetRecordId",
                table: "TaskEntries",
                column: "AssetRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AssetRecordId",
                table: "Tags",
                column: "AssetRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AssetRecords_AssetRecordId",
                table: "Tags",
                column: "AssetRecordId",
                principalTable: "AssetRecords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskEntries_AssetRecords_AssetRecordId",
                table: "TaskEntries",
                column: "AssetRecordId",
                principalTable: "AssetRecords",
                principalColumn: "Id");
        }
    }
}
