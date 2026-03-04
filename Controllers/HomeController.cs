using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeProject.Data;
using RecipeProject.Models;
using RecipeProject.ViewModels;
using System.Diagnostics;

namespace RecipeProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeVM
            {
                FeaturedRecipes = await _context.Recipes
                    .OrderByDescending(r => r.Id)
                    .Take(2)
                    .ToListAsync(),

                BestRecipes = await _context.Recipes
                    .OrderByDescending(r => r.Id)
                    .Skip(2)
                    .Take(6)
                    .ToListAsync()
            };

            return View(vm);
        }

    }
}
