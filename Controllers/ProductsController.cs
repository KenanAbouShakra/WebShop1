using Microsoft.AspNetCore.Mvc;
using WebShop1.Data;
using WebShop1.Models;

namespace WebShop1.Controllers
{
	public class ProductsController : Controller
	{
		private readonly AppDbContext _db;

		public ProductsController(AppDbContext db)
		{
			_db = db;
		}

		public IActionResult Index()
		{
			var products = _db.Products.ToList();
			return View(products);
		}

		// Bare for test: lag et dummy-produkt
		public IActionResult CreateTest()
		{
			var p = new Product { Name = "Test produkt", Price = 99m };
			_db.Products.Add(p);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
