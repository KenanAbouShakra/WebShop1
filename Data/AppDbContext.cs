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
        public DbSet<ProductIngredient> ProductIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductIngredient>()
                .HasKey(pi => new { pi.ProductId, pi.IngredientId });

            builder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductIngredients)
                .HasForeignKey(pi => pi.ProductId);

            builder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Ingredient)
                .WithMany(i => i.ProductIngredients)
                .HasForeignKey(pi => pi.IngredientId);

            // Seed Data
            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Klær" },
                new Category { CategoryId = 2, Name = "Utstyr" },
                new Category { CategoryId = 3, Name = "Møbler" },
                new Category { CategoryId = 4, Name = "Bøker" },
                new Category { CategoryId = 5, Name = "Mat" }
            );

            builder.Entity<Ingredient>().HasData(
    new Ingredient { IngredientId = 1, Name = "Hvetemel" },
    new Ingredient { IngredientId = 2, Name = "Durumhvete" },
    new Ingredient { IngredientId = 3, Name = "Salt" },
    new Ingredient { IngredientId = 4, Name = "Sukker" },
    new Ingredient { IngredientId = 5, Name = "Kakao" },
    new Ingredient { IngredientId = 6, Name = "Kakaosmør" },
    new Ingredient { IngredientId = 7, Name = "Vanilje" },
    new Ingredient { IngredientId = 8, Name = "Egg" },
    new Ingredient { IngredientId = 9, Name = "Melk" },
    new Ingredient { IngredientId = 10, Name = "Smør" },
    new Ingredient { IngredientId = 11, Name = "Olivenolje" },
    new Ingredient { IngredientId = 12, Name = "Vann" }
);

            builder.Entity<Product>().HasData(
    // Klær
    new Product
    {
        ProductId = 1,
        Name = "T-skjorte",
        Description = "Myk bomulls-t-skjorte i god kvalitet.",
        Price = 199m,
        Stock = 50,
        CategoryId = 1
    },
    new Product
    {
        ProductId = 2,
        Name = "Hettegenser",
        Description = "Varm og komfortabel hettegenser.",
        Price = 499m,
        Stock = 30,
        CategoryId = 1
    },

    // Utstyr
    new Product
    {
        ProductId = 3,
        Name = "Treningsbag",
        Description = "Romslig treningsbag med flere lommer.",
        Price = 349m,
        Stock = 40,
        CategoryId = 2
    },
    new Product
    {
        ProductId = 4,
        Name = "Stålflaske",
        Description = "Rustfri drikkeflaske – holder drikken kald i 12 timer.",
        Price = 199m,
        Stock = 70,
        CategoryId = 2
    },

    // Møbler
    new Product
    {
        ProductId = 5,
        Name = "Skrivebord",
        Description = "Moderne skrivebord med god arbeidsflate.",
        Price = 1299m,
        Stock = 15,
        CategoryId = 3
    },
    new Product
    {
        ProductId = 6,
        Name = "Kontorstol",
        Description = "Ergonomisk kontorstol med justerbare funksjoner.",
        Price = 999m,
        Stock = 20,
        CategoryId = 3
    },

    // Bøker
    new Product
    {
        ProductId = 7,
        Name = "Lærebok i C#",
        Description = "En komplett introduksjon til C#-programmering.",
        Price = 399m,
        Stock = 25,
        CategoryId = 4
    },
    new Product
    {
        ProductId = 8,
        Name = "Kokebok for nybegynnere",
        Description = "Enkel kokebok med lette oppskrifter for alle.",
        Price = 299m,
        Stock = 35,
        CategoryId = 4
    },

    // Mat
    new Product
    {
        ProductId = 9,
        Name = "Pasta Fusilli",
        Description = "500 g italiensk pasta laget av durumhvete.",
        Price = 29m,
        Stock = 200,
        CategoryId = 5
    },
    new Product
    {
        ProductId = 10,
        Name = "Mørk sjokolade 70%",
        Description = "Intens mørk sjokolade laget av økologiske kakaobønner.",
        Price = 39m,
        Stock = 150,
        CategoryId = 5
    }
);
            builder.Entity<ProductIngredient>().HasData(
    // Pasta Fusilli (ProductId = 9)
    new ProductIngredient { ProductId = 9, IngredientId = 2 }, // Durumhvete
    new ProductIngredient { ProductId = 9, IngredientId = 3 }, // Salt
    new ProductIngredient { ProductId = 9, IngredientId = 12 }, // Vann (hvis du la inn)

    // Mørk Sjokolade 70% (ProductId = 10)
    new ProductIngredient { ProductId = 10, IngredientId = 5 }, // Kakao
    new ProductIngredient { ProductId = 10, IngredientId = 6 }, // Kakaosmør
    new ProductIngredient { ProductId = 10, IngredientId = 4 }, // Sukker
    new ProductIngredient { ProductId = 10, IngredientId = 7 }  // Vanilje
);


        }
    }
}
