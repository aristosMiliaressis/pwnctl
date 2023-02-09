using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class pwnctl_task_profiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Programs_OwningProgramId",
                table: "AssetRecords");

            migrationBuilder.RenameColumn(
                name: "OwningProgramId",
                table: "AssetRecords",
                newName: "ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_OwningProgramId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_ProgramId");

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "TaskDefinitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaskProfileId",
                table: "Programs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TaskProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskDefinitions_ProfileId",
                table: "TaskDefinitions",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_TaskProfileId",
                table: "Programs",
                column: "TaskProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Programs_ProgramId",
                table: "AssetRecords",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_TaskProfiles_TaskProfileId",
                table: "Programs",
                column: "TaskProfileId",
                principalTable: "TaskProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinitions_TaskProfiles_ProfileId",
                table: "TaskDefinitions",
                column: "ProfileId",
                principalTable: "TaskProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Programs_ProgramId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Programs_TaskProfiles_TaskProfileId",
                table: "Programs");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinitions_TaskProfiles_ProfileId",
                table: "TaskDefinitions");

            migrationBuilder.DropTable(
                name: "TaskProfiles");

            migrationBuilder.DropIndex(
                name: "IX_TaskDefinitions_ProfileId",
                table: "TaskDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_Programs_TaskProfileId",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "TaskDefinitions");

            migrationBuilder.DropColumn(
                name: "TaskProfileId",
                table: "Programs");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "AssetRecords",
                newName: "OwningProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_ProgramId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_OwningProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_Programs_OwningProgramId",
                table: "AssetRecords",
                column: "OwningProgramId",
                principalTable: "Programs",
                principalColumn: "Id");
        }
    }
}
