using Microsoft.AspNetCore.Http;

namespace RecipeProject.Areas.AdminPanel.ViewModels.Recipe
{
    public class UpdateRecipeVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DietType { get; set; }
        public string? Cuisine { get; set; }
        public IFormFile? Image { get; set; }
        public string? ExistingImageUrl { get; set; }
        public List<int> IngredientIds { get; set; } = new();
        public List<string?> Quantities { get; set; } = new();
        public List<IngredientOptionVM> AllIngredients { get; set; } = new();
    }
}