using Microsoft.EntityFrameworkCore;
using WebShop1.Models;

namespace WebShop1.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }

		public DbSet<Product> Products { get; set; }
	}
}
