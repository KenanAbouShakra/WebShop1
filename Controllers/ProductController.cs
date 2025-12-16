using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop1.Data;
using WebShop1.Models;

namespace KenanRestaurant.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ============== LIST ==============
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }

        // ============== ADD/EDIT (GET) ==============
        // id = 0 => Create, id > 0 => Edit
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id = 0)
        {
            await LoadDropdowns();

            if (id == 0)
            {
                ViewBag.Operation = "Create";
                return View(new Product
                {
                    ImageUrl = "default.jpg" // default bilde i /wwwroot/images/default.jpg
                });
            }

            var product = await _db.Products
                .Include(p => p.ProductIngredients)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            ViewBag.Operation = "Update";
            return View(product);
        }

        // ============== ADD/EDIT (POST) ==============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(Product product, int[] ingredientIds)
        {
            await LoadDropdowns();

            // Fjerner krasj hvis ImageUrl er null
            product.ImageUrl ??= "default.jpg";

            if (!ModelState.IsValid)
            {
                ViewBag.Operation = product.ProductId == 0 ? "Create" : "Update";
                return View(product);
            }

            // ----------- Create -----------
            if (product.ProductId == 0)
            {
                // bilde
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                    product.ImageUrl = await SaveImage(product.ImageFile);
                else
                    product.ImageUrl = "default.jpg";

                _db.Products.Add(product);
                await _db.SaveChangesAsync();

                // ingredients (many-to-many)
                if (ingredientIds != null && ingredientIds.Length > 0)
                {
                    foreach (var ingId in ingredientIds.Distinct())
                    {
                        _db.ProductIngredients.Add(new ProductIngredients
                        {
                            ProductId = product.ProductId,
                            IngredientId = ingId
                        });
                    }
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // ----------- Update -----------
            var existing = await _db.Products
                .Include(p => p.ProductIngredients)
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            if (existing == null) return NotFound();

            // fields
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
            existing.CategoryId = product.CategoryId;

            // bilde: bare hvis nytt bilde er lastet opp
            if (product.ImageFile != null && product.ImageFile.Length > 0)
                existing.ImageUrl = await SaveImage(product.ImageFile);

            // ingredients: erstatt alle koblinger
            _db.ProductIngredients.RemoveRange(existing.ProductIngredients);

            if (ingredientIds != null && ingredientIds.Length > 0)
            {
                foreach (var ingId in ingredientIds.Distinct())
                {
                    _db.ProductIngredients.Add(new ProductIngredients
                    {
                        ProductId = existing.ProductId,
                        IngredientId = ingId
                    });
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ============== DELETE ==============
        // DELETE - GET
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // DELETE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int productId)
        {
            var product = await _db.Products
                .Include(p => p.ProductIngredients)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null) return NotFound();

            _db.ProductIngredients.RemoveRange(product.ProductIngredients);
            _db.Products.Remove(product);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // ============== HELPERS ==============
        private async Task LoadDropdowns()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            ViewBag.Ingredients = await _db.Ingredients.ToListAsync();
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            // lagrer i: wwwroot/images
            var uploads = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(uploads, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }
    }
}
