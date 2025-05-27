using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class additiontoFuelRegister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cipNo",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dieselPONumber",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "fuelPump",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "issuanceDate",
                table: "FuelRegisters",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cipNo",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "dieselPONumber",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "fuelPump",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "issuanceDate",
                table: "FuelRegisters");
        }
    }
}
