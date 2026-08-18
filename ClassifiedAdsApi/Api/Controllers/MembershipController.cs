using DataAccess.Services.Membership;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/membership-plans")]
public class MembershipController(IMembershipService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await service.GetActivePlansAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var result = await service.GetPlanAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}
