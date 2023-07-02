using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class task_record_run_count_and_stderr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_task_entries_FoundByTaskId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_task_entries_TaskId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_task_entries_asset_records_RecordId",
                table: "task_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_task_entries_operations_OperationId",
                table: "task_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_task_entries_task_definitions_DefinitionId",
                table: "task_entries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_task_entries",
                table: "task_entries");

            migrationBuilder.RenameTable(
                name: "task_entries",
                newName: "task_records");

            migrationBuilder.RenameIndex(
                name: "IX_task_entries_RecordId",
                table: "task_records",
                newName: "IX_task_records_RecordId");

            migrationBuilder.RenameIndex(
                name: "IX_task_entries_OperationId",
                table: "task_records",
                newName: "IX_task_records_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_task_entries_DefinitionId",
                table: "task_records",
                newName: "IX_task_records_DefinitionId");

            migrationBuilder.AddColumn<int>(
                name: "RunCount",
                table: "task_records",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Stderr",
                table: "task_records",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_task_records",
                table: "task_records",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_task_records_FoundByTaskId",
                table: "asset_records",
                column: "FoundByTaskId",
                principalTable: "task_records",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_task_records_TaskId",
                table: "notifications",
                column: "TaskId",
                principalTable: "task_records",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_task_records_asset_records_RecordId",
                table: "task_records",
                column: "RecordId",
                principalTable: "asset_records",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_task_records_operations_OperationId",
                table: "task_records",
                column: "OperationId",
                principalTable: "operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_task_records_task_definitions_DefinitionId",
                table: "task_records",
                column: "DefinitionId",
                principalTable: "task_definitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_asset_records_task_records_FoundByTaskId",
                table: "asset_records");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_task_records_TaskId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_task_records_asset_records_RecordId",
                table: "task_records");

            migrationBuilder.DropForeignKey(
                name: "FK_task_records_operations_OperationId",
                table: "task_records");

            migrationBuilder.DropForeignKey(
                name: "FK_task_records_task_definitions_DefinitionId",
                table: "task_records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_task_records",
                table: "task_records");

            migrationBuilder.DropColumn(
                name: "RunCount",
                table: "task_records");

            migrationBuilder.DropColumn(
                name: "Stderr",
                table: "task_records");

            migrationBuilder.RenameTable(
                name: "task_records",
                newName: "task_entries");

            migrationBuilder.RenameIndex(
                name: "IX_task_records_RecordId",
                table: "task_entries",
                newName: "IX_task_entries_RecordId");

            migrationBuilder.RenameIndex(
                name: "IX_task_records_OperationId",
                table: "task_entries",
                newName: "IX_task_entries_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_task_records_DefinitionId",
                table: "task_entries",
                newName: "IX_task_entries_DefinitionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_task_entries",
                table: "task_entries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_records_task_entries_FoundByTaskId",
                table: "asset_records",
                column: "FoundByTaskId",
                principalTable: "task_entries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_task_entries_TaskId",
                table: "notifications",
                column: "TaskId",
                principalTable: "task_entries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_task_entries_asset_records_RecordId",
                table: "task_entries",
                column: "RecordId",
                principalTable: "asset_records",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_task_entries_operations_OperationId",
                table: "task_entries",
                column: "OperationId",
                principalTable: "operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_task_entries_task_definitions_DefinitionId",
                table: "task_entries",
                column: "DefinitionId",
                principalTable: "task_definitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
