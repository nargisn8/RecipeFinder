using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeProject.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldWishlistId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WishlistId",
                table: "Recipes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WishlistId",
                table: "Recipes",
                type: "int",
                nullable: true);
        }
    }
}
