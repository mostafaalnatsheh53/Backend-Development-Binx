namespace Day3.Models;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    // Navigation Property
    public Customer Customer { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public decimal Total { get; set; }

    // Navigation Property
    public List<OrderItem> OrderItems { get; set; } = new();
}