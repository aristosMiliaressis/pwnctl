using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class op_scope_one_to_many : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_operations_ScopeId",
                table: "operations");

            migrationBuilder.CreateIndex(
                name: "IX_operations_ScopeId",
                table: "operations",
                column: "ScopeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_operations_ScopeId",
                table: "operations");

            migrationBuilder.CreateIndex(
                name: "IX_operations_ScopeId",
                table: "operations",
                column: "ScopeId",
                unique: true);
        }
    }
}
