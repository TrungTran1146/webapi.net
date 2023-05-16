using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    public partial class identy33 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportProduct_Product_ProductId",
                table: "ImportProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Brand_BrandId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_TypeCar_TypeCarId",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "products");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "products",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Product_TypeCarId",
                table: "products",
                newName: "IX_products_TypeCarId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_BrandId",
                table: "products",
                newName: "IX_products_BrandId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportProduct_products_ProductId",
                table: "ImportProduct",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_Brand_BrandId",
                table: "products",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_TypeCar_TypeCarId",
                table: "products",
                column: "TypeCarId",
                principalTable: "TypeCar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportProduct_products_ProductId",
                table: "ImportProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_products_Brand_BrandId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_TypeCar_TypeCarId",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "Product");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Product",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_products_TypeCarId",
                table: "Product",
                newName: "IX_Product_TypeCarId");

            migrationBuilder.RenameIndex(
                name: "IX_products_BrandId",
                table: "Product",
                newName: "IX_Product_BrandId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportProduct_Product_ProductId",
                table: "ImportProduct",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Brand_BrandId",
                table: "Product",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_TypeCar_TypeCarId",
                table: "Product",
                column: "TypeCarId",
                principalTable: "TypeCar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
