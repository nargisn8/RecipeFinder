using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeProject.Migrations
{
    /// <inheritdoc />
    public partial class Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Wishlists_WishlistId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_WishlistId",
                table: "Recipes");

            migrationBuilder.CreateTable(
                name: "RecipeWishlist",
                columns: table => new
                {
                    RecipesId = table.Column<int>(type: "int", nullable: false),
                    WishlistsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeWishlist", x => new { x.RecipesId, x.WishlistsId });
                    table.ForeignKey(
                        name: "FK_RecipeWishlist_Recipes_RecipesId",
                        column: x => x.RecipesId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeWishlist_Wishlists_WishlistsId",
                        column: x => x.WishlistsId,
                        principalTable: "Wishlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeWishlist_WishlistsId",
                table: "RecipeWishlist",
                column: "WishlistsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeWishlist");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_WishlistId",
                table: "Recipes",
                column: "WishlistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Wishlists_WishlistId",
                table: "Recipes",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "Id");
        }
    }
}
