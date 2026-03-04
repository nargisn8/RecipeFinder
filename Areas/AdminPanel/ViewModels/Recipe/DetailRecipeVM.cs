namespace RecipeProject.Areas.AdminPanel.ViewModels.Recipe
{
    public class DetailRecipeVM
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DietType { get; set; }
        public string? Cuisine { get; set; }
        public string? ImageUrl { get; set; }
        public List<RecipeIngredientVM> Ingredients { get; set; } = new();
    }

    public class RecipeIngredientVM
    {
        public string? IngredientName { get; set; }
        public string? Quantity { get; set; }
    }

}
