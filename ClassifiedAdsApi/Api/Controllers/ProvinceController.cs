using DataAccess.Services.Province;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/provinces")]
public class ProvinceController(IProvinceService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await service.GetActiveAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var result = await service.GetDetailsAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}
