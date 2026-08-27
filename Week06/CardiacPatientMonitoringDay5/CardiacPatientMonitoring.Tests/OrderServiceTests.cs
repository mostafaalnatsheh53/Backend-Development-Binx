using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Models;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Tests;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenStockIsAvailable_CreatesOrderAndDecreasesStock()
    {
        await using var db = CreateDatabase();
        db.Patients.Add(new Patient { Id = 1, FirstName = "Alex", LastName = "Taylor" });
        db.Products.Add(new Product { Id = 10, Name = "Monitor", UnitPrice = 125.50m, StockQuantity = 5 });
        await db.SaveChangesAsync();

        var response = await new OrderService(db).CreateAsync(new CreateOrderRequestDto
        {
            CustomerId = 1,
            Items = [new CreateOrderItemDto { ProductId = 10, Quantity = 2 }]
        });

        Assert.Equal(1, response.Id);
        Assert.Equal(2, response.Items.Single().Quantity);
        Assert.Equal(125.50m, response.Items.Single().UnitPrice);
        Assert.Equal(251.00m, response.Items.Single().LineTotal);
        Assert.Equal(251.00m, response.OrderTotal);
        Assert.Equal(3, (await db.Products.FindAsync(10))!.StockQuantity);
        Assert.Equal(251.00m, (await db.OrderItems.SingleAsync()).LineTotal);
        Assert.Single(db.Orders);
    }

    [Fact]
    public async Task CreateAsync_WhenAnyItemHasInsufficientStock_DoesNotChangeStockOrCreateOrder()
    {
        await using var db = CreateDatabase();
        db.Patients.Add(new Patient { Id = 1, FirstName = "Alex", LastName = "Taylor" });
        db.Products.AddRange(
            new Product { Id = 10, Name = "Monitor", UnitPrice = 100m, StockQuantity = 5 },
            new Product { Id = 11, Name = "Sensor", UnitPrice = 25m, StockQuantity = 1 });
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(() => new OrderService(db).CreateAsync(
            new CreateOrderRequestDto
            {
                CustomerId = 1,
                Items =
                [
                    new CreateOrderItemDto { ProductId = 10, Quantity = 2 },
                    new CreateOrderItemDto { ProductId = 11, Quantity = 2 }
                ]
            }));

        Assert.Empty(db.Orders);
        Assert.Equal(5, (await db.Products.FindAsync(10))!.StockQuantity);
        Assert.Equal(1, (await db.Products.FindAsync(11))!.StockQuantity);
    }

    [Fact]
    public async Task CreateAsync_WhenDuplicateProductItemsAreRequested_SumsQuantitiesBeforeCheckingStock()
    {
        await using var db = CreateDatabase();
        db.Patients.Add(new Patient { Id = 1, FirstName = "Alex", LastName = "Taylor" });
        db.Products.Add(new Product { Id = 10, Name = "Monitor", UnitPrice = 100m, StockQuantity = 3 });
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(() => new OrderService(db).CreateAsync(
            new CreateOrderRequestDto
            {
                CustomerId = 1,
                Items =
                [
                    new CreateOrderItemDto { ProductId = 10, Quantity = 2 },
                    new CreateOrderItemDto { ProductId = 10, Quantity = 2 }
                ]
            }));

        Assert.Empty(db.Orders);
        Assert.Equal(3, (await db.Products.FindAsync(10))!.StockQuantity);
    }

    private static ApplicationDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}