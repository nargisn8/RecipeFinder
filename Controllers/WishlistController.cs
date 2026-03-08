using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeProject.Data;

namespace RecipeProject.Controllers
{
    public class WishlistController : Controller
    {
        //private readonly AppDbContext _context;

        //public WishlistController(AppDbContext context)
        //{
        //    _context = context;
        //}

        //[HttpPost("AddRecipe")]
        //public async Task<IActionResult> AddRecipeToWishlist(int recipeId, int wishlistId)
        //{
        //    // 1. Resepti mövcud Wishlist kolleksiyası ilə birlikdə tapırıq
        //    var recipe = await _context.Recipes
        //        .Include(r => r.Wishlists)
        //        .FirstOrDefaultAsync(r => r.Id == recipeId);

        //    // 2. Wishlist-i tapırıq
        //    var wishlist = await _context.Wishlists.FindAsync(wishlistId);

        //    // 3. Yoxlamalar
        //    if (recipe == null) return NotFound("Resept tapılmadı.");
        //    if (wishlist == null) return NotFound("Wishlist tapılmadı.");

        //    // 4. Əgər artıq əlavə olunubsa, təkrar əlavə etməyək
        //    if (recipe.Wishlists.Any(w => w.Id == wishlistId))
        //    {
        //        return BadRequest("Bu resept artıq bu siyahıda var.");
        //    }

        //    // 5. Əlaqəni yaradırıq
        //    recipe.Wishlists.Add(wishlist);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //        return Ok("Resept uğurla əlavə edildi.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Xəta baş verdi: {ex.Message}");
        //    }
        //}

        //[HttpGet("{wishlistId}")]
        //public async Task<IActionResult> GetWishlistWithRecipes(int wishlistId)
        //{
        //    var wishlist = await _context.Wishlists
        //        .Include(w => w.Recipes)
        //        .FirstOrDefaultAsync(w => w.Id == wishlistId);

        //    if (wishlist == null) return NotFound();

        //    return Ok(wishlist);
        //}
    }
}
