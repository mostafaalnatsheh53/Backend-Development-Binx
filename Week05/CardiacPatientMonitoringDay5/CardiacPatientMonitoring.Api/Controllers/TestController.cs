using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("exception")]
    public IActionResult TriggerException()
    {
        throw new InvalidOperationException(
            "This is a controlled exception for middleware testing.");
    }
}