namespace RecipeProject.Areas.AdminPanel.ViewModels.Recipe
{
    public class GetRecipeVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? DietType { get; set; }
        public string? Cuisine { get; set; }
        public string? ImageUrl { get; set; }
    }
}
