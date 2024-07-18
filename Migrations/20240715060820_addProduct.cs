using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace MVCApplication.Migrations
{
    /// <inheritdoc />
    public partial class addProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_AuthorId",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Posts",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.CreateTable(
                name: "CategoryProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryProducts_CategoryProducts_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "CategoryProducts",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Slug = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true),
                    Content = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Published = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    AuthorId = table.Column<string>(type: "varchar(255)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductCategoryProducts",
                columns: table => new
                {
                    CategoryProductId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategoryProducts", x => new { x.ProductId, x.CategoryProductId });
                    table.ForeignKey(
                        name: "FK_ProductCategoryProducts_CategoryProducts_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CategoryProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategoryProducts_Products_PostId",
                        column: x => x.PostId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProducts_ParentCategoryId",
                table: "CategoryProducts",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProducts_Slug",
                table: "CategoryProducts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryProducts_CategoryId",
                table: "ProductCategoryProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryProducts_PostId",
                table: "ProductCategoryProducts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_AuthorId",
                table: "Products",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Slug",
                table: "Products",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_AuthorId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "ProductCategoryProducts");

            migrationBuilder.DropTable(
                name: "CategoryProducts");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Posts",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
