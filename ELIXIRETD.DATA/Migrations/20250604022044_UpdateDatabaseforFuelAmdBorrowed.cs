using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class UpdateDatabaseforFuelAmdBorrowed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessUnitCode",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessUnitName",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentUnitCode",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentUnitName",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OneChargingName",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubUnitCode",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubUnitName",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessUnitCode",
                table: "BorrowedConsumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessUnitName",
                table: "BorrowedConsumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentUnitCode",
                table: "BorrowedConsumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentUnitName",
                table: "BorrowedConsumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OneChargingName",
                table: "BorrowedConsumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubUnitCode",
                table: "BorrowedConsumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubUnitName",
                table: "BorrowedConsumes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessUnitCode",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "BusinessUnitName",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "DepartmentUnitCode",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "DepartmentUnitName",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "OneChargingName",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "SubUnitCode",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "SubUnitName",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "BusinessUnitCode",
                table: "BorrowedConsumes");

            migrationBuilder.DropColumn(
                name: "BusinessUnitName",
                table: "BorrowedConsumes");

            migrationBuilder.DropColumn(
                name: "DepartmentUnitCode",
                table: "BorrowedConsumes");

            migrationBuilder.DropColumn(
                name: "DepartmentUnitName",
                table: "BorrowedConsumes");

            migrationBuilder.DropColumn(
                name: "OneChargingName",
                table: "BorrowedConsumes");

            migrationBuilder.DropColumn(
                name: "SubUnitCode",
                table: "BorrowedConsumes");

            migrationBuilder.DropColumn(
                name: "SubUnitName",
                table: "BorrowedConsumes");
        }
    }
}
