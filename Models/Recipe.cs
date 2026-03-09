using RecipeProject.Models.Base;

namespace RecipeProject.Models
{
    public class Recipe : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string DietType { get; set; } = "None";
        public string? Cuisine { get; set; }
        public string? ImageUrl { get; set; }
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
        public List<Wishlist> Wishlists { get; set; } = new();
    }
}
