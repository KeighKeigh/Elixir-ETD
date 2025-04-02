using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class createAssetSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetNo = table.Column<int>(type: "int", nullable: true),
                    AssetCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusSync = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Manual = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}
