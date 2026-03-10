using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeProject.Areas.AdminPanel.ViewModels.Recipe;
using RecipeProject.Data;
using RecipeProject.Helpers;
using RecipeProject.Helpers.Enum;
using RecipeProject.Models;

namespace RecipeProject.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.Member)}")]
    public class RecipeController : Controller
    {
        private readonly AppDbContext _context;

        public RecipeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Recipes
                .Where(r => !r.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim().ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(searchString) ||
                                         r.Cuisine.ToLower().Contains(searchString));
            }

            var recipes = await query
                .OrderByDescending(r => r.Id)
                .Select(r => new GetRecipeVM
                {
                    Id = r.Id,
                    Name = r.Name,
                    DietType = r.DietType,
                    Cuisine = r.Cuisine,
                    ImageUrl = r.ImageUrl
                })
                .ToListAsync();

            ViewBag.CurrentSearch = searchString;

            return View(recipes);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (recipe == null) return NotFound();

            var detailVM = new DetailRecipeVM
            {
                Name = recipe.Name,
                Description = recipe.Description,
                DietType = recipe.DietType,
                Cuisine = recipe.Cuisine,
                ImageUrl = recipe.ImageUrl,
                Ingredients = recipe.RecipeIngredients.Select(ri => new RecipeIngredientVM
                {
                    IngredientName = ri.Ingredient.Name,
                    Quantity = ri.Quantity
                }).ToList()
            };

            return View(detailVM);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateRecipeVM
            {
                AllIngredients = await GetAllIngredientsAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRecipeVM createRecipeVM)
        {
            createRecipeVM.AllIngredients = await GetAllIngredientsAsync();

            if (!ModelState.IsValid)
                return View(createRecipeVM);

            var name = (createRecipeVM.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "Name is required.");
                return View(createRecipeVM);
            }

            if (await _context.Recipes.AnyAsync(r => r.Name.Trim().ToLower() == name.ToLower() && !r.IsDeleted))
            {
                ModelState.AddModelError("Name", "This recipe already exists.");
                return View(createRecipeVM);
            }

            string? imageUrl = null;
            if (createRecipeVM.Image != null)
            {
                if (!FileValidator.IsImage(createRecipeVM.Image))
                {
                    ModelState.AddModelError("Image", "Only image files are allowed.");
                    return View(createRecipeVM);
                }

                if (!FileValidator.IsValidSize(createRecipeVM.Image, 5))
                {
                    ModelState.AddModelError("Image", "Image size must be less than 5MB.");
                    return View(createRecipeVM);
                }

                imageUrl = await FileValidator.SaveFileAsync(createRecipeVM.Image, "recipes");
            }

            var recipe = new Recipe
            {
                Name = name,
                Description = createRecipeVM.Description?.Trim(),
                DietType = createRecipeVM.DietType ?? "None",
                Cuisine = createRecipeVM.Cuisine?.Trim(),
                ImageUrl = imageUrl
            };

            for (int i = 0; i < createRecipeVM.IngredientIds.Count; i++)
            {
                recipe.RecipeIngredients.Add(new RecipeIngredient
                {
                    IngredientId = createRecipeVM.IngredientIds[i],
                    Quantity = i < createRecipeVM.Quantities.Count
                        ? createRecipeVM.Quantities[i]
                        : null
                });
            }

            try
            {
                await _context.Recipes.AddAsync(recipe);
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
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (recipe == null) return NotFound();

            var vm = new UpdateRecipeVM
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                DietType = recipe.DietType,
                Cuisine = recipe.Cuisine,
                ExistingImageUrl = recipe.ImageUrl,
                IngredientIds = recipe.RecipeIngredients.Select(ri => ri.IngredientId).ToList(),
                Quantities = recipe.RecipeIngredients.Select(ri => ri.Quantity).ToList(),
                AllIngredients = await GetAllIngredientsAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateRecipeVM updateRecipeVM)
        {
            updateRecipeVM.AllIngredients = await GetAllIngredientsAsync();

            if (!ModelState.IsValid)
                return View(updateRecipeVM);

            var name = (updateRecipeVM.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "Name is required.");
                return View(updateRecipeVM);
            }

            if (await _context.Recipes.AnyAsync(r => r.Name.Trim().ToLower() == name.ToLower()
                && r.Id != updateRecipeVM.Id && !r.IsDeleted))
            {
                ModelState.AddModelError("Name", "This recipe already exists.");
                return View(updateRecipeVM);
            }

            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.Id == updateRecipeVM.Id);

            if (recipe == null) return NotFound();

            try
            {
                recipe.Name = name;
                recipe.Description = updateRecipeVM.Description?.Trim();
                recipe.DietType = updateRecipeVM.DietType ?? "None";
                recipe.Cuisine = updateRecipeVM.Cuisine?.Trim();

                if (updateRecipeVM.Image != null)
                {
                    if (!FileValidator.IsImage(updateRecipeVM.Image))
                    {
                        ModelState.AddModelError("Image", "Only image files are allowed.");
                        return View(updateRecipeVM);
                    }

                    if (!FileValidator.IsValidSize(updateRecipeVM.Image, 5))
                    {
                        ModelState.AddModelError("Image", "Image size must be less than 5MB.");
                        return View(updateRecipeVM);
                    }

                    FileValidator.DeleteFile(recipe.ImageUrl);

                    recipe.ImageUrl = await FileValidator.SaveFileAsync(updateRecipeVM.Image, "recipes");
                }

                _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);

                for (int i = 0; i < updateRecipeVM.IngredientIds.Count; i++)
                {
                    recipe.RecipeIngredients.Add(new RecipeIngredient
                    {
                        IngredientId = updateRecipeVM.IngredientIds[i],
                        Quantity = i < updateRecipeVM.Quantities.Count
                            ? updateRecipeVM.Quantities[i]
                            : null
                    });
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Content(ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Delete(int id)
        {
            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (recipe == null) return NotFound();

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null) return NotFound();

            try
            {
                recipe.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Content(ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<IngredientOptionVM>> GetAllIngredientsAsync()
        {
            return await _context.Ingredients
                .Where(i => !i.IsDeleted)
                .Select(i => new IngredientOptionVM
                {
                    Id = i.Id,
                    Name = i.Name
                })
                .ToListAsync();
        }
    }
}