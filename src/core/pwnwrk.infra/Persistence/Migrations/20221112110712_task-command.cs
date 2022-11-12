using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnwrk.infra.Persistence.Migrations
{
    public partial class taskcommand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Arguments",
                table: "TaskRecords",
                newName: "Discriminator");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Discriminator",
                table: "TaskRecords",
                newName: "Arguments");
        }
    }
}
