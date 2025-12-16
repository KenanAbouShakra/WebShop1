using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebShop1.Data;
using WebShop1.Models;

namespace WebShop1.Controllers
{
    public class OrderController : Controller
    {
        private const string SessionKey = "OrderViewModel";

        private readonly AppDbContext _appDbContext;
        private readonly Repository<Product> _products;
        private readonly Repository<Order> _orders;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _appDbContext = context;
            _userManager = userManager;
            _products = new Repository<Product>(context);
            _orders = new Repository<Order>(context);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = GetFromSession() ?? new OrderViewModel
            {
                OrederItems = new List<OrderItemViewModel>(),
                TTotalAmount = 0
            };

            // lister ikke null
            model.OrederItems ??= new List<OrderItemViewModel>();

            // Produkter hentes alltid ferskt til siden
            model.Products = await _products.GetAllAsync();

            // Recalculate total (i tilfelle session-data mangler eller har endret seg)
            model.TTotalAmount = model.OrederItems.Sum(x => x.Price * x.Quantity);

            SaveToSession(model);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddItem(int prodId, int prodQty)
        {
            if (prodQty <= 0) prodQty = 1;

            var product = await _appDbContext.Products.FindAsync(prodId);
            if (product == null)
                return NotFound();

            var model = GetFromSession() ?? new OrderViewModel
            {
                OrederItems = new List<OrderItemViewModel>(),
                TTotalAmount = 0
            };

            model.OrederItems ??= new List<OrderItemViewModel>();

            var productName = GetProductName(product);
            var productPrice = GetProductPrice(product);

            var existing = model.OrederItems.FirstOrDefault(x => x.ProductId == prodId);
            if (existing != null)
            {
                existing.Quantity += prodQty;

                // behold samme pris, men hvis den er 0 så sett den
                if (existing.Price <= 0)
                    existing.Price = productPrice;
            }
            else
            {
                model.OrederItems.Add(new OrderItemViewModel
                {
                    ProductId = prodId,
                    ProductName = productName,
                    Quantity = prodQty,
                    Price = productPrice
                });
            }

            model.TTotalAmount = model.OrederItems.Sum(x => x.Price * x.Quantity);

            SaveToSession(model);
            return RedirectToAction(nameof(Create));
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveItem(int prodId)
        {
            var model = GetFromSession();
            if (model?.OrederItems == null)
                return RedirectToAction(nameof(Create));

            model.OrederItems.RemoveAll(x => x.ProductId == prodId);
            model.TTotalAmount = model.OrederItems.Sum(x => x.Price * x.Quantity);

            SaveToSession(model);
            return RedirectToAction(nameof(Create));
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CompleteOrder()
        {
            var model = GetFromSession();
            if (model == null || model.OrederItems == null || !model.OrederItems.Any())
                return RedirectToAction(nameof(Create));

            var user = await _userManager.GetUserAsync(User);

            var order = new Order
            {
                UserId = user!.Id,
                OrderDate = DateTime.Now,
                TotalAmount = model.TTotalAmount,
                OrderItems = model.OrederItems.Select(i => new OrderItem
                {
                    PrudetId = i.ProductId,   // bruker dine feltnavn
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            _appDbContext.Orders.Add(order);
            await _appDbContext.SaveChangesAsync();

            HttpContext.Session.Remove(SessionKey);

            return RedirectToAction(nameof(MyOrders));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = await _appDbContext.Orders
                .Where(o => o.UserId == user!.Id)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }


        // -------- Session helpers (slipper å være avhengig av SessionExtension) --------

        private OrderViewModel? GetFromSession()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.Deserialize<OrderViewModel>(json);
        }

        private void SaveToSession(OrderViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            HttpContext.Session.SetString(SessionKey, json);
        }

        // -------- Produkt helpers (funker selv om Product bruker litt andre property-navn) --------

        private static string GetProductName(Product product)
        {
            // prøver vanlige property-navn: Name / ProductName / Title
            return (product.GetType().GetProperty("Name")?.GetValue(product)?.ToString())
                ?? (product.GetType().GetProperty("ProductName")?.GetValue(product)?.ToString())
                ?? (product.GetType().GetProperty("Title")?.GetValue(product)?.ToString())
                ?? "Ukjent produkt";
        }

        private static decimal GetProductPrice(Product product)
        {
            // prøver vanlige property-navn: Price / UnitPrice
            var priceObj =
                product.GetType().GetProperty("Price")?.GetValue(product)
                ?? product.GetType().GetProperty("UnitPrice")?.GetValue(product);

            return priceObj is decimal d ? d : 0m;
        }
    }
}
