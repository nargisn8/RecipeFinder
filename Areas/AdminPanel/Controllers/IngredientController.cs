using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using RecipeProject.Areas.AdminPanel.ViewModels.Ingredient;
using RecipeProject.Data;
using RecipeProject.Models;

namespace RecipeProject.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class IngredientController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<IngredientController> _logger;

        public IngredientController(AppDbContext context, ILogger<IngredientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Ingredients
                .AsNoTracking()
                .OrderBy(i => i.Name)
                .Select(i => new GetIngredientVM
                {
                    Id = i.Id,
                    Name = i.Name
                })
                .ToListAsync();

            return View(items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIngredientVM createIngredientVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createIngredientVM);
            }

            var name = (createIngredientVM?.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(createIngredientVM.Name), "Name is required.");
                return View(createIngredientVM);
            }

            bool existIngredient = await _context.Ingredients
                .AnyAsync(c => c.Name.Trim().ToLower() == name.ToLower());

            if (existIngredient)
            {
                ModelState.AddModelError("Name", "This ingredient already exists");
                return View(createIngredientVM);
            }

            try
            {
                var ingredient = new Ingredient
                {
                    Name = name
                };
                await _context.Ingredients.AddAsync(ingredient);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Content(ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            var updateVM = new UpdateIngredientVM
            {
                Id = ingredient.Id,
                Name = ingredient.Name
            };

            return View(updateVM);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateIngredientVM updateIngredientVM)
        {
            if (!ModelState.IsValid)
            {
                return View(updateIngredientVM);
            }

            var name = (updateIngredientVM?.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(updateIngredientVM.Name), "Name is required.");
                return View(updateIngredientVM);
            }

            // Eyni adda başqa ingredient var mı? (özü istisna)
            bool existIngredient = await _context.Ingredients
                .AnyAsync(c => c.Name.Trim().ToLower() == name.ToLower() && c.Id != updateIngredientVM.Id);

            if (existIngredient)
            {
                ModelState.AddModelError("Name", "This ingredient already exists");
                return View(updateIngredientVM);
            }

            var ingredient = await _context.Ingredients.FindAsync(updateIngredientVM.Id);

            if (ingredient == null)
            {
                return NotFound();
            }

            try
            {
                ingredient.Name = name;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Content(ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            try
            {
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Content(ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            var detailVM = new DetailIngredientVM
            {
                Name = ingredient.Name
            };

            return View(detailVM);
        }
    }
}
