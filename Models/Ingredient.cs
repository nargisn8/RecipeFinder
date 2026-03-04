using RecipeProject.Models.Base;

namespace RecipeProject.Models
{
    public class Ingredient : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
    }
}
