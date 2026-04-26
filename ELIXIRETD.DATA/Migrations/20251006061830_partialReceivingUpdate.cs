using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class partialReceivingUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemRemarks",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemRemarks",
                table: "PoSummaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YmirId",
                table: "PoSummaries",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemRemarks",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "ItemRemarks",
                table: "PoSummaries");

            migrationBuilder.DropColumn(
                name: "YmirId",
                table: "PoSummaries");
        }
    }
}
