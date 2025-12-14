using Microsoft.AspNetCore.Mvc;
using WebShop1.Data;
using WebShop1.Models;

namespace WebShop1.Controllers
{
    public class IngredientController : Controller
    {
        private Repository<Ingredient> ingredients;
    public IngredientController(AppDbContext context)
        {
            ingredients = new Repository<Ingredient>(context);
        }
        public async Task<IActionResult> Index()
        {
            return View(await ingredients.GetAllAsync());
        }
        public async Task<IActionResult> Details(int id)
        {
            return View(await ingredients.GetByIdAsync(id, new QueryOptions<Ingredient>() { Includes="ProductIngredients.Product"}));
        }
        [HttpGet]
        public IActionResult Create() {
            return View();
        }
        [HttpPost]
        [ ValidateAntiForgeryToken]
        public async Task<IActionResult>Create([Bind("Ingredient, Name")] Ingredient ingredient)
        {
            if(ModelState.IsValid)
            {
                await ingredients.AddAsync(ingredient);
                return RedirectToAction("Index");
            }
            return View(ingredient);
        }
    }
}
