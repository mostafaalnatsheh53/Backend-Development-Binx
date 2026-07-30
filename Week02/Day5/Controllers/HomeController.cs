using Microsoft.AspNetCore.Mvc;
using MiddlewareDemo.Services;

namespace MiddlewareDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly GreetingService _greetingService;

    public HomeController(GreetingService greetingService)
    {
        _greetingService = greetingService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_greetingService.GetGreeting());
    }
}