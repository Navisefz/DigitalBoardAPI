using Microsoft.EntityFrameworkCore.Migrations;

namespace TV_DASH_API.Migrations
{
    public partial class ClarkFloorChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClarkFloor",
                table: "TVDash_ClarkImages");

            migrationBuilder.AddColumn<int>(
                name: "Floor",
                table: "TVDash_ClarkImages",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Floor",
                table: "TVDash_ClarkImages");

            migrationBuilder.AddColumn<int>(
                name: "ClarkFloor",
                table: "TVDash_ClarkImages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
