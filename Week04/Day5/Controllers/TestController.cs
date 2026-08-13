using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Day3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        return Ok("This endpoint is public.");
    }

    [HttpGet("protected")]
    [Authorize]
    public IActionResult Protected()
    {
        return Ok("You are authenticated.");
    }
}