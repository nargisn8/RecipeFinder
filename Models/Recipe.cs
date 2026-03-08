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


        // Wishlist ilə əlaqə
        public int? WishlistId { get; set; }  // Bu resept hansı wishlist-ə aiddir
        public List<Wishlist> Wishlists { get; set; } = new();
    }
}
