using RecipeProject.Models.Base;

namespace RecipeProject.Models
{
    public class Wishlist : BaseEntity
    {
        public string UserId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public List<Recipe> Recipes { get; set; } = new();
    }
}
