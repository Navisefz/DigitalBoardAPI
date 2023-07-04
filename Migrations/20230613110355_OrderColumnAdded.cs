using Microsoft.EntityFrameworkCore.Migrations;

namespace TV_DASH_API.Migrations
{
    public partial class OrderColumnAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "TVDash_CebuImages",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "TVDash_CebuImages");
        }
    }
}
