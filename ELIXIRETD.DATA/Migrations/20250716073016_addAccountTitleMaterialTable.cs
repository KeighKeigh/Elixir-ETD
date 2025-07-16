using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addAccountTitleMaterialTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountMats");

            migrationBuilder.CreateTable(
                name: "AccountTitleMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountTitleId = table.Column<int>(type: "int", nullable: true),
                    MaterialId = table.Column<int>(type: "int", nullable: true),
                    MaterialNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTitleMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountTitleMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountTitleMaterials_OneAccountTitles_AccountTitleId",
                        column: x => x.AccountTitleId,
                        principalTable: "OneAccountTitles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountTitleMaterials_AccountTitleId",
                table: "AccountTitleMaterials",
                column: "AccountTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTitleMaterials_MaterialId",
                table: "AccountTitleMaterials",
                column: "MaterialId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountTitleMaterials");

            migrationBuilder.CreateTable(
                name: "AccountMats",
                columns: table => new
                {
                    AccountTitleId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false),
                    MaterialNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountMats", x => x.AccountTitleId);
                    table.ForeignKey(
                        name: "FK_AccountMats_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountMats_OneAccountTitles_AccountTitleId",
                        column: x => x.AccountTitleId,
                        principalTable: "OneAccountTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountMats_MaterialId",
                table: "AccountMats",
                column: "MaterialId");
        }
    }
}
