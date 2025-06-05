using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class UpdateMiscTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "business_unit_code",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "business_unit_name",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department_unit_code",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department_unit_name",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "one_charging_name",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sub_unit_code",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sub_unit_name",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "business_unit_code",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "business_unit_name",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department_unit_code",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department_unit_name",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "one_charging_name",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sub_unit_code",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sub_unit_name",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "business_unit_code",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "business_unit_name",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "department_unit_code",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "department_unit_name",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "one_charging_name",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "sub_unit_code",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "sub_unit_name",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "business_unit_code",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "business_unit_name",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "department_unit_code",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "department_unit_name",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "one_charging_name",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "sub_unit_code",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "sub_unit_name",
                table: "MiscellaneousIssues");
        }
    }
}
