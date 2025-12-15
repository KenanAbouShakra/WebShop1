using Microsoft.AspNetCore.Mvc;
using WebShop1.Data;
using WebShop1.Models;

namespace TequliasRestaurant.Controllers
{
	public class ProductController : Controller
	{
		private Repository<Product> products;

		public ProductController(AppDbContext db)
		{
			this.products = new Repository<Product>(db);
		}

		public async Task<IActionResult> Index()
		{
			return View(await products.GetAllAsync());
		}
	}
}
