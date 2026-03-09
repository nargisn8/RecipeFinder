using RecipeProject.Models;

namespace RecipeProject.ViewModels
{
    public class RecipesVM
    {

        public int? IngredientId { get; set; }
        public string? Search { get; set; }
        public string? Cuisine { get; set; }
        public string? DietType { get; set; }

        public List<Ingredient> Ingredients { get; set; } = new();
        public List<Recipe> Recipes { get; set; } = new();
        public List<string> Cuisines { get; set; } = new();
        public List<string> DietTypes { get; set; } = new();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
