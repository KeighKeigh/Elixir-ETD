using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class RemoveMaterialAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialAccountTitles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialAccountTitles",
                columns: table => new
                {
                    AccountTitleId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialAccountTitles", x => x.AccountTitleId);
                    table.ForeignKey(
                        name: "FK_MaterialAccountTitles_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialAccountTitles_OneAccountTitles_AccountTitleId",
                        column: x => x.AccountTitleId,
                        principalTable: "OneAccountTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAccountTitles_MaterialId",
                table: "MaterialAccountTitles",
                column: "MaterialId");
        }
    }
}
