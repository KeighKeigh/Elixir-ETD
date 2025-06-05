using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class AddOneCharginToBorrowedAndFuel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OneChargingCode",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OneChargingCode",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OneChargingCode",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OneChargingCode",
                table: "BorrowedConsumes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OneChargingCode",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "OneChargingCode",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "OneChargingCode",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "OneChargingCode",
                table: "BorrowedConsumes");
        }
    }
}
