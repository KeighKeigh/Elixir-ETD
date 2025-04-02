using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class updateAssetInFuelRegister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asset",
                table: "FuelRegisters");

            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "FuelRegisters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelRegisters_AssetId",
                table: "FuelRegisters",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelRegisters_Assets_AssetId",
                table: "FuelRegisters",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelRegisters_Assets_AssetId",
                table: "FuelRegisters");

            migrationBuilder.DropIndex(
                name: "IX_FuelRegisters_AssetId",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "FuelRegisters");

            migrationBuilder.AddColumn<string>(
                name: "Asset",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
} 
