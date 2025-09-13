using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class AddDecimalPricesForSyncYmir : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PriceWithDecimal",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "PoSummaries",
                type: "decimal(38,20)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Billed",
                table: "PoSummaries",
                type: "decimal(38,16)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "PriceWithDecimal",
                table: "PoSummaries",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceWithDecimal",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "PriceWithDecimal",
                table: "PoSummaries");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "PoSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,20)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Billed",
                table: "PoSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,16)");
        }
    }
}
