using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Day1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    // Only Admin users can access this endpoint.
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult GetAdminData()
    {
        return Ok("Only admins can access this endpoint.");
    }

    // Requires the CanManageOrders policy.
    [Authorize(Policy = "CanManageOrders")]
    [HttpGet("manage-orders")]
    public IActionResult ManageOrders()
    {
        return Ok("You can manage orders.");
    }
}