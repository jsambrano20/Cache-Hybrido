using Microsoft.EntityFrameworkCore;
using hybrid_cache.Entities;

namespace hybrid_cache.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().HasData(
                new Produto { Id = 1, Nome = "Produto 1", Preco = 99.99m, Estoque = 50 },
                new Produto { Id = 2, Nome = "Produto 2", Preco = 150.00m, Estoque = 30 },
                new Produto { Id = 3, Nome = "Produto 3", Preco = 299.50m, Estoque = 20 },
                new Produto { Id = 4, Nome = "Produto 4", Preco = 299.50m, Estoque = 12 },
                new Produto { Id = 5, Nome = "Produto 5", Preco = 55.50m, Estoque = 33 }
            );

            base.OnModelCreating(modelBuilder);

        }
    }
}
