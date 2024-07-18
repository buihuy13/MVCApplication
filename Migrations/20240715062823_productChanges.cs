using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCApplication.Migrations
{
    /// <inheritdoc />
    public partial class productChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategoryProducts_CategoryProducts_CategoryId",
                table: "ProductCategoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategoryProducts_Products_PostId",
                table: "ProductCategoryProducts");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategoryProducts_CategoryId",
                table: "ProductCategoryProducts");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategoryProducts_PostId",
                table: "ProductCategoryProducts");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ProductCategoryProducts");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "ProductCategoryProducts");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryProducts_CategoryProductId",
                table: "ProductCategoryProducts",
                column: "CategoryProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategoryProducts_CategoryProducts_CategoryProductId",
                table: "ProductCategoryProducts",
                column: "CategoryProductId",
                principalTable: "CategoryProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategoryProducts_Products_ProductId",
                table: "ProductCategoryProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategoryProducts_CategoryProducts_CategoryProductId",
                table: "ProductCategoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategoryProducts_Products_ProductId",
                table: "ProductCategoryProducts");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategoryProducts_CategoryProductId",
                table: "ProductCategoryProducts");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ProductCategoryProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "ProductCategoryProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryProducts_CategoryId",
                table: "ProductCategoryProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryProducts_PostId",
                table: "ProductCategoryProducts",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategoryProducts_CategoryProducts_CategoryId",
                table: "ProductCategoryProducts",
                column: "CategoryId",
                principalTable: "CategoryProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategoryProducts_Products_PostId",
                table: "ProductCategoryProducts",
                column: "PostId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
