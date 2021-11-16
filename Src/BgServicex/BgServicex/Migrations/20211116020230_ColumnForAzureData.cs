using Microsoft.EntityFrameworkCore.Migrations;

namespace BgServicex.Migrations
{
    public partial class ColumnForAzureData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "EventFiles",
                newName: "DirectoryInAzure");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DirectoryInAzure",
                table: "EventFiles",
                newName: "CreationTime");
        }
    }
}
