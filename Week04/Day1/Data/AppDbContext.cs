using Day1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Day1.Data;
// This class represents the application's database context, which is responsible for managing the database connection and providing access to the entities in the application.
public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }
    // Add DbSet properties for your entities here.
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.Total)
            .HasPrecision(18, 2);
    }
}