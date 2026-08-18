using DataAccess.Services.City;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/cities")]
public class CityController(ICityService service) : ControllerBase
{
    [HttpGet("by-province/{provinceId:int}")]
    public async Task<IActionResult> ByProvince(int provinceId)
        => Ok(await service.GetByProvinceAsync(provinceId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var result = await service.GetDetailsAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}
