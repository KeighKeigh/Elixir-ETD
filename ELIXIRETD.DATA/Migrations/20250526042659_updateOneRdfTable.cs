using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class updateOneRdfTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessUnit",
                table: "OneRdfs");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OneRdfs",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "OneRdfs",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "OneRdfs",
                newName: "sync_id");

            migrationBuilder.RenameColumn(
                name: "SyncId",
                table: "OneRdfs",
                newName: "sub_unit_id");

            migrationBuilder.RenameColumn(
                name: "SubUnit",
                table: "OneRdfs",
                newName: "location_id");

            migrationBuilder.RenameColumn(
                name: "LocationName",
                table: "OneRdfs",
                newName: "sub_unit_name");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "OneRdfs",
                newName: "department_unit_id");

            migrationBuilder.RenameColumn(
                name: "LocationCode",
                table: "OneRdfs",
                newName: "sub_unit_code");

            migrationBuilder.RenameColumn(
                name: "DepartmentName",
                table: "OneRdfs",
                newName: "location_code");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "OneRdfs",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "DepartmentCode",
                table: "OneRdfs",
                newName: "locationN_nme");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "OneRdfs",
                newName: "department_unit_name");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "OneRdfs",
                newName: "business_unit_id");

            migrationBuilder.RenameColumn(
                name: "CompanyCode",
                table: "OneRdfs",
                newName: "department_unit_code");

            migrationBuilder.AddColumn<string>(
                name: "business_unit_code",
                table: "OneRdfs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "business_unit_name",
                table: "OneRdfs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "company_code",
                table: "OneRdfs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "company_name",
                table: "OneRdfs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "OneRdfs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department_code",
                table: "OneRdfs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department_id",
                table: "OneRdfs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department_name",
                table: "OneRdfs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "business_unit_code",
                table: "OneRdfs");

            migrationBuilder.DropColumn(
                name: "business_unit_name",
                table: "OneRdfs");

            migrationBuilder.DropColumn(
                name: "company_code",
                table: "OneRdfs");

            migrationBuilder.DropColumn(
                name: "company_name",
                table: "OneRdfs");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "OneRdfs");

            migrationBuilder.DropColumn(
                name: "department_code",
                table: "OneRdfs");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "OneRdfs");

            migrationBuilder.DropColumn(
                name: "department_name",
                table: "OneRdfs");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "OneRdfs",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "OneRdfs",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "sync_id",
                table: "OneRdfs",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "sub_unit_name",
                table: "OneRdfs",
                newName: "LocationName");

            migrationBuilder.RenameColumn(
                name: "sub_unit_id",
                table: "OneRdfs",
                newName: "SyncId");

            migrationBuilder.RenameColumn(
                name: "sub_unit_code",
                table: "OneRdfs",
                newName: "LocationCode");

            migrationBuilder.RenameColumn(
                name: "location_id",
                table: "OneRdfs",
                newName: "SubUnit");

            migrationBuilder.RenameColumn(
                name: "location_code",
                table: "OneRdfs",
                newName: "DepartmentName");

            migrationBuilder.RenameColumn(
                name: "locationN_nme",
                table: "OneRdfs",
                newName: "DepartmentCode");

            migrationBuilder.RenameColumn(
                name: "department_unit_name",
                table: "OneRdfs",
                newName: "CompanyName");

            migrationBuilder.RenameColumn(
                name: "department_unit_id",
                table: "OneRdfs",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "department_unit_code",
                table: "OneRdfs",
                newName: "CompanyCode");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "OneRdfs",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "business_unit_id",
                table: "OneRdfs",
                newName: "CompanyId");

            migrationBuilder.AddColumn<int>(
                name: "BusinessUnit",
                table: "OneRdfs",
                type: "int",
                nullable: true);
        }
    }
}
