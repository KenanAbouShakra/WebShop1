using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebShop1.Models;

namespace WebShop1.Data
{
	public class AppDbContext : IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ProductIngredients> ProductIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // OrderItem -> Product (FK hos deg heter PrudetId)
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.PrudetId);

            // OrderItem -> Order (navigation hos deg heter Oreder)
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Oreder)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);


            builder.Entity<ProductIngredients>()
                .HasKey(pi => new { pi.ProductId, pi.IngredientId });

            builder.Entity<ProductIngredients>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductIngredients)
                .HasForeignKey(pi => pi.ProductId);

            builder.Entity<ProductIngredients>()
                .HasOne(pi => pi.Ingredient)
                .WithMany(i => i.ProductIngredients)
                .HasForeignKey(pi => pi.IngredientId);


            // Seed Data
            builder.Entity<Category>().HasData(
     new Category { CategoryId = 1, Name = "Forretter" },
     new Category { CategoryId = 2, Name = "Hovedretter" },
     new Category { CategoryId = 3, Name = "Desserter" },
     new Category { CategoryId = 4, Name = "Drikke" }
 );

            builder.Entity<Ingredient>().HasData(
                new Ingredient { IngredientId = 1, Name = "Kylling" },
                new Ingredient { IngredientId = 2, Name = "Ris" },
                new Ingredient { IngredientId = 3, Name = "Pasta" },
                new Ingredient { IngredientId = 4, Name = "Fløte" },
                new Ingredient { IngredientId = 5, Name = "Parmesan" },
                new Ingredient { IngredientId = 6, Name = "Tomat" },
                new Ingredient { IngredientId = 7, Name = "Laks" },
                new Ingredient { IngredientId = 8, Name = "Potet" },
                new Ingredient { IngredientId = 9, Name = "Sjokolade" },
                new Ingredient { IngredientId = 10, Name = "Vanilje" }
            );


            builder.Entity<Product>().HasData(
     new Product
     {
         ProductId = 1,
         Name = "Kylling Alfredo",
         Description = "Pasta med kremet fløtesaus og grillet kylling",
         Price = 159m,
         Stock = 999, // betyr egentlig ikke noe for restaurant
         CategoryId = 2
     },
     new Product
     {
         ProductId = 2,
         Name = "Laks med Sitron",
         Description = "Ovnsbakt laks med poteter og frisk sitronsaus",
         Price = 189m,
         Stock = 999,
         CategoryId = 2
     },
     new Product
     {
         ProductId = 3,
         Name = "Tomatsuppe",
         Description = "Hjemmelaget tomatsuppe med basilikum",
         Price = 89m,
         Stock = 999,
         CategoryId = 1
     },
     new Product
     {
         ProductId = 4,
         Name = "Sjokoladekake",
         Description = "Fyldig sjokoladekake servert med vaniljeis",
         Price = 79m,
         Stock = 999,
         CategoryId = 3
     }
 );
            builder.Entity<ProductIngredients>().HasData(
                // Kylling Alfredo
                new ProductIngredients { ProductId = 1, IngredientId = 1 },
                new ProductIngredients { ProductId = 1, IngredientId = 3 },
                new ProductIngredients { ProductId = 1, IngredientId = 4 },
                new ProductIngredients { ProductId = 1, IngredientId = 5 },

                // Laks med sitron
                new ProductIngredients { ProductId = 2, IngredientId = 7 },
                new ProductIngredients { ProductId = 2, IngredientId = 8 },

                // Tomatsuppe
                new ProductIngredients { ProductId = 3, IngredientId = 6 },

                // Sjokoladekake
                new ProductIngredients { ProductId = 4, IngredientId = 9 },
                new ProductIngredients { ProductId = 4, IngredientId = 10 }
            );



        }
    }
}
