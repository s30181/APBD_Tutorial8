using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClients()
    {
        return Ok(new List<TripDTO>());
    }
}