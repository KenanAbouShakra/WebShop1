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

        // ================= LIST =================
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }

        // ================= CREATE (GET) =================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            ViewBag.Operation = "Create";
            return View(new Product());
        }

        // ================= CREATE (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, int[] ingredientIds)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                ViewBag.Operation = "Create";
                return View(product);
            }

            // ---------- IMAGE ----------
            if (product.ImageFile != null)
            {
                product.ImageUrl = await SaveImage(product.ImageFile);
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            // ---------- INGREDIENTS ----------
            if (ingredientIds != null)
            {
                foreach (var ingId in ingredientIds)
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

        // ================= EDIT (GET) =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products
                .Include(p => p.ProductIngredients)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            await LoadDropdowns();
            ViewBag.Operation = "Edit";
            return View(product);
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, int[] ingredientIds)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                ViewBag.Operation = "Edit";
                return View(product);
            }

            var existing = await _db.Products
                .Include(p => p.ProductIngredients)
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            if (existing == null) return NotFound();

            // ---------- UPDATE FIELDS ----------
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
            existing.CategoryId = product.CategoryId;

            // ---------- IMAGE ----------
            if (product.ImageFile != null)
            {
                existing.ImageUrl = await SaveImage(product.ImageFile);
            }

            // ---------- INGREDIENTS ----------
            _db.ProductIngredients.RemoveRange(existing.ProductIngredients);

            if (ingredientIds != null)
            {
                foreach (var ingId in ingredientIds)
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

        // ================= DELETE (GET) =================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // ================= DELETE (POST) =================
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

        // ================= HELPERS =================
        private async Task LoadDropdowns()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            ViewBag.Ingredients = await _db.Ingredients.ToListAsync();
        }

        private async Task<string> SaveImage(IFormFile file)
        {
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
