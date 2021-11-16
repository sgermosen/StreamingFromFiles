using Microsoft.EntityFrameworkCore.Migrations;

namespace BgServicex.Migrations
{
    public partial class ModifyingNameToPReserveOriginalData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "EventFiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "EventFiles");
        }
    }
}
