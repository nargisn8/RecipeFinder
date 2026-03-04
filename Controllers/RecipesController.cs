using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeProject.Data;
using RecipeProject.Models;
using RecipeProject.ViewModels;

namespace RecipeProject.Controllers
{
    public class RecipesController : Controller
    {
        private readonly AppDbContext _context;

        public RecipesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? ingredientId, string? search, string? cuisine, string? dietType)
        {
            IQueryable<Recipe> recipes = _context.Recipes
                .Where(r => !r.IsDeleted)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient);

            // Ingredient ID ilə filter
            if (ingredientId.HasValue)
            {
                recipes = recipes.Where(r =>
                    r.RecipeIngredients.Any(ri => ri.IngredientId == ingredientId));
            }

            // Search: ingredient adına görə axtarış
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                recipes = recipes.Where(r =>
                    r.RecipeIngredients.Any(ri =>
                        ri.Ingredient.Name.ToLower().Contains(term)));
            }

            // Cuisine filter
            if (!string.IsNullOrWhiteSpace(cuisine))
            {
                recipes = recipes.Where(r => r.Cuisine == cuisine);
            }

            // DietType filter
            if (!string.IsNullOrWhiteSpace(dietType))
            {
                recipes = recipes.Where(r => r.DietType == dietType);
            }

            var vm = new RecipesVM
            {
                IngredientId = ingredientId,
                Search = search,
                Cuisine = cuisine,
                DietType = dietType,
                Ingredients = await _context.Ingredients
                    .Where(i => !i.IsDeleted)
                    .ToListAsync(),
                Recipes = await recipes.ToListAsync(),
                Cuisines = await _context.Recipes
                    .Where(r => !r.IsDeleted && r.Cuisine != null)
                    .Select(r => r.Cuisine!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync(),
                DietTypes = await _context.Recipes
                    .Where(r => !r.IsDeleted && r.DietType != null && r.DietType != "None")
                    .Select(r => r.DietType!)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}