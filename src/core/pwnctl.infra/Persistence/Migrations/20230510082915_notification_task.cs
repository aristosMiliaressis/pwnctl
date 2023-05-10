using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class notification_task : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_notification_rules_RuleId",
                table: "notifications");

            migrationBuilder.AlterColumn<int>(
                name: "RuleId",
                table: "notifications",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "notifications",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_TaskId",
                table: "notifications",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_notification_rules_RuleId",
                table: "notifications",
                column: "RuleId",
                principalTable: "notification_rules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_task_entries_TaskId",
                table: "notifications",
                column: "TaskId",
                principalTable: "task_entries",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_notification_rules_RuleId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_task_entries_TaskId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_TaskId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "notifications");

            migrationBuilder.AlterColumn<int>(
                name: "RuleId",
                table: "notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_notification_rules_RuleId",
                table: "notifications",
                column: "RuleId",
                principalTable: "notification_rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
