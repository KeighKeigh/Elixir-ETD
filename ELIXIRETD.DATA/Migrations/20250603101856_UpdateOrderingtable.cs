using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class UpdateOrderingtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "business_unit_code",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "business_unit_name",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department_unit_code",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department_unit_name",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "one_charging_name",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sub_unit_code",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sub_unit_name",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "business_unit_code",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "business_unit_name",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "department_unit_code",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "department_unit_name",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "one_charging_name",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "sub_unit_code",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "sub_unit_name",
                table: "Orders");
        }
    }
}
