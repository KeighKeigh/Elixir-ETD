using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class MaterialAndAccountTitlePivot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialAccountTitles",
                columns: table => new
                {
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    AccountTitleId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialAccountTitles", x => new { x.MaterialId, x.AccountTitleId });
                    table.ForeignKey(
                        name: "FK_MaterialAccountTitles_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialAccountTitles_OneAccountTitles_AccountTitleId",
                        column: x => x.AccountTitleId,
                        principalTable: "OneAccountTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAccountTitles_AccountTitleId",
                table: "MaterialAccountTitles",
                column: "AccountTitleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialAccountTitles");
        }
    }
}
