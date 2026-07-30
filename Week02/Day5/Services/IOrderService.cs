namespace MiddlewareDemo.Services;

public interface IOrderService
{
    List<string> GetOrders();
    string GetOrderById(int id);
    string CreateOrder(string customerName);
}