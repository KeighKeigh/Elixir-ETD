using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class updateFuelRegisterandDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dieselPONumber",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "fuelPump",
                table: "FuelRegisters");

            migrationBuilder.AddColumn<string>(
                name: "dieselPONumber",
                table: "FuelRegisterDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "fuelPump",
                table: "FuelRegisterDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dieselPONumber",
                table: "FuelRegisterDetails");

            migrationBuilder.DropColumn(
                name: "fuelPump",
                table: "FuelRegisterDetails");

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
        }
    }
}
