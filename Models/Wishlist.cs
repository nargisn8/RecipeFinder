using RecipeProject.Models.Base;

namespace RecipeProject.Models
{
    public class Wishlist : BaseEntity
    {
        public string UserId { get; set; }
        public List<Recipe> Recipes { get; set; } = new();
    }
}
