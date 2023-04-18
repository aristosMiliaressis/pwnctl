using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Migrations
{
    public partial class ShortName_vo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "TaskProfiles",
                newName: "ShortName_Value");

            migrationBuilder.RenameColumn(
                name: "SubjectClass_Class",
                table: "TaskDefinitions",
                newName: "SubjectClass_Value");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "TaskDefinitions",
                newName: "ShortName_Value");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "ScopeAggregates",
                newName: "ShortName_Value");

            migrationBuilder.RenameColumn(
                name: "AllowActive",
                table: "Policies",
                newName: "OnlyPassive");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Operations",
                newName: "ShortName_Value");

            migrationBuilder.RenameColumn(
                name: "SubjectClass_Class",
                table: "NotificationRules",
                newName: "SubjectClass_Value");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "NotificationRules",
                newName: "ShortName_Value");

            migrationBuilder.RenameColumn(
                name: "SubjectClass_Class",
                table: "AssetRecords",
                newName: "SubjectClass_Value");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortName_Value",
                table: "TaskProfiles",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "SubjectClass_Value",
                table: "TaskDefinitions",
                newName: "SubjectClass_Class");

            migrationBuilder.RenameColumn(
                name: "ShortName_Value",
                table: "TaskDefinitions",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "ShortName_Value",
                table: "ScopeAggregates",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "OnlyPassive",
                table: "Policies",
                newName: "AllowActive");

            migrationBuilder.RenameColumn(
                name: "ShortName_Value",
                table: "Operations",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SubjectClass_Value",
                table: "NotificationRules",
                newName: "SubjectClass_Class");

            migrationBuilder.RenameColumn(
                name: "ShortName_Value",
                table: "NotificationRules",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "SubjectClass_Value",
                table: "AssetRecords",
                newName: "SubjectClass_Class");
        }
    }
}
