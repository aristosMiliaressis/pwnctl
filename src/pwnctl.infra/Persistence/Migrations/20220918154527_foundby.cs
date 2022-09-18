using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pwnctl.infra.Persistence.Migrations
{
    public partial class foundby : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRoutable",
                table: "VirtualHosts");

            migrationBuilder.DropColumn(
                name: "FoundAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "FoundAt",
                table: "TaskDefinitions");

            migrationBuilder.DropColumn(
                name: "FoundAt",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IsRoutable",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "FoundAt",
                table: "ScopeDefinitions");

            migrationBuilder.DropColumn(
                name: "FoundAt",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "IsRoutable",
                table: "Parameters");

            migrationBuilder.DropColumn(
                name: "FoundAt",
                table: "OperationalPolicies");

            migrationBuilder.DropColumn(
                name: "FoundAt",
                table: "NotificationRules");

            migrationBuilder.DropColumn(
                name: "FoundAt",
                table: "NotificationProviderSettings");

            migrationBuilder.DropColumn(
                name: "FoundAt",
                table: "NotificationChannels");

            migrationBuilder.DropColumn(
                name: "IsRoutable",
                table: "NetRanges");

            migrationBuilder.DropColumn(
                name: "IsRoutable",
                table: "Keywords");

            migrationBuilder.DropColumn(
                name: "IsRoutable",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "IsRoutable",
                table: "Endpoints");

            migrationBuilder.DropColumn(
                name: "IsRoutable",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "IsRoutable",
                table: "DNSRecords");

            migrationBuilder.AddColumn<string>(
                name: "FoundBy",
                table: "VirtualHosts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoundBy",
                table: "Services",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoundBy",
                table: "Parameters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoundBy",
                table: "NetRanges",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoundBy",
                table: "Keywords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoundBy",
                table: "Hosts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoundBy",
                table: "Endpoints",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoundBy",
                table: "Domains",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FoundBy",
                table: "DNSRecords",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoundBy",
                table: "VirtualHosts");

            migrationBuilder.DropColumn(
                name: "FoundBy",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "FoundBy",
                table: "Parameters");

            migrationBuilder.DropColumn(
                name: "FoundBy",
                table: "NetRanges");

            migrationBuilder.DropColumn(
                name: "FoundBy",
                table: "Keywords");

            migrationBuilder.DropColumn(
                name: "FoundBy",
                table: "Hosts");

            migrationBuilder.DropColumn(
                name: "FoundBy",
                table: "Endpoints");

            migrationBuilder.DropColumn(
                name: "FoundBy",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "FoundBy",
                table: "DNSRecords");

            migrationBuilder.AddColumn<bool>(
                name: "IsRoutable",
                table: "VirtualHosts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FoundAt",
                table: "Tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FoundAt",
                table: "TaskDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FoundAt",
                table: "Tags",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsRoutable",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FoundAt",
                table: "ScopeDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FoundAt",
                table: "Programs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsRoutable",
                table: "Parameters",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FoundAt",
                table: "OperationalPolicies",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FoundAt",
                table: "NotificationRules",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FoundAt",
                table: "NotificationProviderSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FoundAt",
                table: "NotificationChannels",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsRoutable",
                table: "NetRanges",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRoutable",
                table: "Keywords",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRoutable",
                table: "Hosts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRoutable",
                table: "Endpoints",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRoutable",
                table: "Domains",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRoutable",
                table: "DNSRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
