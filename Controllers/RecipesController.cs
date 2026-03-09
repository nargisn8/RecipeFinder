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

        public async Task<IActionResult> Index(int? ingredientId, string? search, string? cuisine, string? dietType, int page = 1)
        {
            int pageSize = 6;

            IQueryable<Recipe> query = _context.Recipes
                .Where(r => !r.IsDeleted)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient);

            if (ingredientId.HasValue)
            {
                query = query.Where(r => r.RecipeIngredients.Any(ri => ri.IngredientId == ingredientId));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(term) ||
                                        r.RecipeIngredients.Any(ri => ri.Ingredient.Name.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(cuisine))
            {
                query = query.Where(r => r.Cuisine == cuisine);
            }

            if (!string.IsNullOrWhiteSpace(dietType))
            {
                query = query.Where(r => r.DietType == dietType);
            }

            var totalRecipes = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecipes / (double)pageSize);

            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var recipesForPage = await query
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new RecipesVM
            {
                IngredientId = ingredientId,
                Search = search,
                Cuisine = cuisine,
                DietType = dietType,

                CurrentPage = page,
                TotalPages = totalPages,

                Ingredients = await _context.Ingredients.Where(i => !i.IsDeleted).ToListAsync(),
                Recipes = recipesForPage,

                Cuisines = await _context.Recipes
                    .Where(r => !r.IsDeleted && r.Cuisine != null)
                    .Select(r => r.Cuisine!).Distinct().OrderBy(c => c).ToListAsync(),

                DietTypes = await _context.Recipes
                    .Where(r => !r.IsDeleted && r.DietType != null && r.DietType != "None")
                    .Select(r => r.DietType!).Distinct().OrderBy(d => d).ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddToWishlist(int recipeId)
        {
            var userId = User.Identity.Name;


            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please log in first." });
            }

            var wishlist = _context.Wishlists.FirstOrDefault(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId };
                _context.Wishlists.Add(wishlist);
                _context.SaveChanges();
            }

            var recipe = _context.Recipes
                .Include(r => r.Wishlists)
                .FirstOrDefault(r => r.Id == recipeId);

            if (recipe != null)
            {
                if (!recipe.Wishlists.Any(w => w.Id == wishlist.Id))
                {
                    recipe.Wishlists.Add(wishlist);
                    _context.SaveChanges();
                }
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "No recipes found." });
        }

        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recipe == null) return NotFound();
            return View(recipe);
        }
    }
}