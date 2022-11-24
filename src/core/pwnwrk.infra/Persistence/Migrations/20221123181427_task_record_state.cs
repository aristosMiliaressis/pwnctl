using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnwrk.infra.Persistence.Migrations
{
    public partial class task_record_state : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "TaskRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "TaskRecords");
        }
    }
}
