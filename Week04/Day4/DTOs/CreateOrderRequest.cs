namespace Day1.DTOs;

public class CreateOrderRequest
{
    public int CustomerId { get; set; }
    public decimal Total { get; set; }
    public DateTime OrderDate { get; set; }
}