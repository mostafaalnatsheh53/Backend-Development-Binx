namespace MiddlewareDemo.Services;

public class GreetingService
{
    public GreetingService()
    {
        Console.WriteLine("GreetingService Created");
    }

    public string GetGreeting()
    {
        return "Hello from GreetingService!";
    }
}