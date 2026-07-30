namespace MiddlewareDemo.Services;

public class OrderService : IOrderService
{
    private readonly List<string> _orders = new()
    {
        "Laptop",
        "Mouse",
        "Keyboard"
    };

    public List<string> GetOrders()
    {
        return _orders;
    }

    public string GetOrderById(int id)
    {
        if (id <= 0 || id > _orders.Count)
            return "Order Not Found";

        return _orders[id - 1];
    }

    public string CreateOrder(string customerName)
    {
        string order = $"Order for {customerName}";
        _orders.Add(order);
        return $"Created: {order}";
    }
}