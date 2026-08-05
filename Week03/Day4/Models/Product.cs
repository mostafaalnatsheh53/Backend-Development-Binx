namespace Day4.Models;
public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    // Navigation Property
    public List<OrderItem> OrderItems { get; set; } = new();
}