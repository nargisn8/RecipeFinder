using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeProject.Migrations
{
    /// <inheritdoc />
    public partial class AddStr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DietType",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DietType",
                table: "Recipes");
        }
    }
}
