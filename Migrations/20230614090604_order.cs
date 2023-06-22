using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    public partial class order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalOder",
                table: "Order",
                newName: "Quantity");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOrder",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalOrder",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Order",
                newName: "TotalOder");
        }
    }
}
