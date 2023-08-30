using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class policy_refactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_policies_task_profiles_TaskProfileId",
                table: "policies");

            migrationBuilder.DropIndex(
                name: "IX_policies_TaskProfileId",
                table: "policies");

            migrationBuilder.DropColumn(
                name: "Aggressiveness",
                table: "task_definitions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "task_definitions");

            migrationBuilder.DropColumn(
                name: "MaxAggressiveness",
                table: "policies");

            migrationBuilder.DropColumn(
                name: "OnlyPassive",
                table: "policies");

            migrationBuilder.DropColumn(
                name: "TaskProfileId",
                table: "policies");

            migrationBuilder.DropColumn(
                name: "Whitelist",
                table: "policies");

            migrationBuilder.CreateTable(
                name: "policy_task_profiles",
                columns: table => new
                {
                    PolicyId = table.Column<int>(type: "integer", nullable: false),
                    TaskProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_policy_task_profiles", x => new { x.PolicyId, x.TaskProfileId });
                    table.ForeignKey(
                        name: "FK_policy_task_profiles_policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_policy_task_profiles_task_profiles_TaskProfileId",
                        column: x => x.TaskProfileId,
                        principalTable: "task_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_policy_task_profiles_TaskProfileId",
                table: "policy_task_profiles",
                column: "TaskProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "policy_task_profiles");

            migrationBuilder.AddColumn<int>(
                name: "Aggressiveness",
                table: "task_definitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "task_definitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "MaxAggressiveness",
                table: "policies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "OnlyPassive",
                table: "policies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TaskProfileId",
                table: "policies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Whitelist",
                table: "policies",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_policies_TaskProfileId",
                table: "policies",
                column: "TaskProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_policies_task_profiles_TaskProfileId",
                table: "policies",
                column: "TaskProfileId",
                principalTable: "task_profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
