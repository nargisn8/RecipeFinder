using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RecipeFinder.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
