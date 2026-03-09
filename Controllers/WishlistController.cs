using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeProject.Data;
using RecipeProject.Models;

namespace RecipeProject.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddRecipe(int recipeId)
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return Json(new { success = false, message = "Please login first." });
            }

            // 1. İstifadəçinin wishlist-ini tapırıq və ya yoxdursa yaradırıq
            var wishlist = await _context.Wishlists
                .Include(w => w.Recipes)
                .FirstOrDefaultAsync(w => w.UserId == userName);

            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userName, Recipes = new List<Recipe>() };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            // 2. Resepti tapırıq
            var recipe = await _context.Recipes
                .Include(r => r.Wishlists)
                .FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe == null) return Json(new { success = false, message = "Recipe not found." });

            // 3. Əlaqəni yoxlayırıq və əlavə edirik
            if (!wishlist.Recipes.Any(r => r.Id == recipeId))
            {
                wishlist.Recipes.Add(recipe);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Recipe added successfully!" });
            }

            return Json(new { success = false, message = "Already in your wishlist." });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.Recipes)
                .FirstOrDefaultAsync(w => w.UserId == userName);

            if (wishlist == null)
            {
                wishlist = new Wishlist { Recipes = new List<Recipe>() };
            }

            return View(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRecipe(int recipeId)
        {
            var userName = User.Identity.Name;
            var wishlist = await _context.Wishlists
                .Include(w => w.Recipes)
                .FirstOrDefaultAsync(w => w.UserId == userName);

            var recipe = wishlist?.Recipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe != null)
            {
                wishlist.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
