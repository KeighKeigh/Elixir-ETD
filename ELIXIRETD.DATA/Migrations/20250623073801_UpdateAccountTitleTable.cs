using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class UpdateAccountTitleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "code",
                table: "OneAccountTitles",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "Delete",
                table: "OneAccountTitles",
                newName: "NormalBalance");

            migrationBuilder.RenameColumn(
                name: "AccountTitleName",
                table: "OneAccountTitles",
                newName: "FinancialStatement");

            migrationBuilder.RenameColumn(
                name: "AccountTitleCode",
                table: "OneAccountTitles",
                newName: "Charging");

            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "OneAccountTitles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountDescription",
                table: "OneAccountTitles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountGroup",
                table: "OneAccountTitles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountSubgroup",
                table: "OneAccountTitles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "OneAccountTitles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Allocation",
                table: "OneAccountTitles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OneAccountTitles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "OneAccountTitles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OneAccountTitles",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "OneAccountTitles");

            migrationBuilder.DropColumn(
                name: "AccountDescription",
                table: "OneAccountTitles");

            migrationBuilder.DropColumn(
                name: "AccountGroup",
                table: "OneAccountTitles");

            migrationBuilder.DropColumn(
                name: "AccountSubgroup",
                table: "OneAccountTitles");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "OneAccountTitles");

            migrationBuilder.DropColumn(
                name: "Allocation",
                table: "OneAccountTitles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OneAccountTitles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "OneAccountTitles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OneAccountTitles");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "OneAccountTitles",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "NormalBalance",
                table: "OneAccountTitles",
                newName: "Delete");

            migrationBuilder.RenameColumn(
                name: "FinancialStatement",
                table: "OneAccountTitles",
                newName: "AccountTitleName");

            migrationBuilder.RenameColumn(
                name: "Charging",
                table: "OneAccountTitles",
                newName: "AccountTitleCode");
        }
    }
}
