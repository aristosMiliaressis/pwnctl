using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class operations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_Programs_ProgramId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_ScopeDefinitions_Programs_ProgramId",
                table: "ScopeDefinitions");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "OperationalPolicies");

            migrationBuilder.DropIndex(
                name: "IX_ScopeDefinitions_ProgramId",
                table: "ScopeDefinitions");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "ScopeDefinitions");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "AssetRecords",
                newName: "ScopeId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_ProgramId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_ScopeId");

            migrationBuilder.AddColumn<int>(
                name: "OperationId",
                table: "TaskEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Blacklist = table.Column<string>(type: "text", nullable: true),
                    Whitelist = table.Column<string>(type: "text", nullable: true),
                    MaxAggressiveness = table.Column<long>(type: "bigint", nullable: true),
                    AllowActive = table.Column<bool>(type: "boolean", nullable: false),
                    TaskProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Policies_TaskProfiles_TaskProfileId",
                        column: x => x.TaskProfileId,
                        principalTable: "TaskProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScopeAggregates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeAggregates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    PolicyId = table.Column<int>(type: "integer", nullable: true),
                    ScopeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Operations_ScopeAggregates_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "ScopeAggregates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScopeDefinitionAggregates",
                columns: table => new
                {
                    AggregateId = table.Column<int>(type: "integer", nullable: false),
                    DefinitionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeDefinitionAggregates", x => new { x.AggregateId, x.DefinitionId });
                    table.ForeignKey(
                        name: "FK_ScopeDefinitionAggregates_ScopeAggregates_AggregateId",
                        column: x => x.AggregateId,
                        principalTable: "ScopeAggregates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScopeDefinitionAggregates_ScopeDefinitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "ScopeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskEntries_OperationId",
                table: "TaskEntries",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_PolicyId",
                table: "Operations",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_ScopeId",
                table: "Operations",
                column: "ScopeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policies_TaskProfileId",
                table: "Policies",
                column: "TaskProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeDefinitionAggregates_DefinitionId",
                table: "ScopeDefinitionAggregates",
                column: "DefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetRecords_ScopeDefinitions_ScopeId",
                table: "AssetRecords",
                column: "ScopeId",
                principalTable: "ScopeDefinitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskEntries_Operations_OperationId",
                table: "TaskEntries",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetRecords_ScopeDefinitions_ScopeId",
                table: "AssetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskEntries_Operations_OperationId",
                table: "TaskEntries");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "ScopeDefinitionAggregates");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "ScopeAggregates");

            migrationBuilder.DropIndex(
                name: "IX_TaskEntries_OperationId",
                table: "TaskEntries");

            migrationBuilder.DropColumn(
                name: "OperationId",
                table: "TaskEntries");

            migrationBuilder.RenameColumn(
                name: "ScopeId",
                table: "AssetRecords",
                newName: "ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetRecords_ScopeId",
                table: "AssetRecords",
                newName: "IX_AssetRecords_ProgramId");

            migrationBuilder.AddColumn<int>(
                name: "ProgramId",
                table: "ScopeDefinitions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OperationalPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AllowActive = table.Column<bool>(type: "boolean", nullable: false),
                    Blacklist = table.Column<string>(type: "text", nullable: true),
                    MaxAggressiveness = table.Column<long>(type: "bigint", nullable: true),
                    Whitelist = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationalPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PolicyId = table.Column<int>(type: "integer", nullable: true),
                    TaskProfileId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Platform = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_OperationalPolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "OperationalPolicies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Programs_TaskProfiles_TaskProfileId",
                        column: x => x.TaskProfileId,
                        principalTable: "TaskProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScopeDefinitions_ProgramId",
                table: "ScopeDefinitions",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_PolicyId",
                table: "Programs",
                column: "PolicyId",
                unique: true);

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
                name: "FK_ScopeDefinitions_Programs_ProgramId",
                table: "ScopeDefinitions",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
