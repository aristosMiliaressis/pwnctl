using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class notification_uniqness : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_RecordId",
                table: "notifications");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_RecordId_RuleId",
                table: "notifications",
                columns: new[] { "RecordId", "RuleId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_RecordId_RuleId",
                table: "notifications");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_RecordId",
                table: "notifications",
                column: "RecordId");
        }
    }
}
