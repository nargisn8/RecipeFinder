using RecipeProject.Models;

namespace RecipeProject.ViewModels
{
    public class HomeVM
    {
        public List<Recipe> FeaturedRecipes { get; set; } = new();
        public List<Recipe> BestRecipes { get; set; } = new();
    }
}
