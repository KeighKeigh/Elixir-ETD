using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addAccountMat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountMats",
                columns: table => new
                {
                    AccountTitleId = table.Column<int>(type: "int", nullable: false),
                    MaterialNo = table.Column<int>(type: "int", nullable: true),
                    MaterialId = table.Column<int>(type: "int", nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountMats");
        }
    }
}
