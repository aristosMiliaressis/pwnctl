using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Persistence.Migrations
{
    public partial class url : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parameters_EndpointId_Name_Type",
                table: "Parameters");

            migrationBuilder.RenameColumn(
                name: "Uri",
                table: "Endpoints",
                newName: "Url");

            migrationBuilder.RenameIndex(
                name: "IX_Endpoints_Uri",
                table: "Endpoints",
                newName: "IX_Endpoints_Url");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Parameters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_EndpointId",
                table: "Parameters",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_Url_Name_Type",
                table: "Parameters",
                columns: new[] { "Url", "Name", "Type" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parameters_EndpointId",
                table: "Parameters");

            migrationBuilder.DropIndex(
                name: "IX_Parameters_Url_Name_Type",
                table: "Parameters");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Parameters");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Endpoints",
                newName: "Uri");

            migrationBuilder.RenameIndex(
                name: "IX_Endpoints_Url",
                table: "Endpoints",
                newName: "IX_Endpoints_Uri");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_EndpointId_Name_Type",
                table: "Parameters",
                columns: new[] { "EndpointId", "Name", "Type" },
                unique: true);
        }
    }
}
