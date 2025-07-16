using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class removeMaterialonMaterialAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialAccountTitles_Materials_MaterialId",
                table: "MaterialAccountTitles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialAccountTitles",
                table: "MaterialAccountTitles");

            migrationBuilder.DropIndex(
                name: "IX_MaterialAccountTitles_AccountTitleId",
                table: "MaterialAccountTitles");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialId",
                table: "MaterialAccountTitles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialAccountTitles",
                table: "MaterialAccountTitles",
                column: "AccountTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAccountTitles_MaterialId",
                table: "MaterialAccountTitles",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialAccountTitles_Materials_MaterialId",
                table: "MaterialAccountTitles",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialAccountTitles_Materials_MaterialId",
                table: "MaterialAccountTitles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialAccountTitles",
                table: "MaterialAccountTitles");

            migrationBuilder.DropIndex(
                name: "IX_MaterialAccountTitles_MaterialId",
                table: "MaterialAccountTitles");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialId",
                table: "MaterialAccountTitles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialAccountTitles",
                table: "MaterialAccountTitles",
                columns: new[] { "MaterialId", "AccountTitleId" });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialAccountTitles_AccountTitleId",
                table: "MaterialAccountTitles",
                column: "AccountTitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialAccountTitles_Materials_MaterialId",
                table: "MaterialAccountTitles",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
