using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    public partial class upcart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Cart",
                newName: "Image");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Cart",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Cart");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Cart",
                newName: "Status");
        }
    }
}
