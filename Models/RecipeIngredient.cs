using RecipeProject.Models.Base;

namespace RecipeProject.Models
{
    public class RecipeIngredient : BaseEntity
    {
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;
        public string? Quantity { get; set; }

    }
}
