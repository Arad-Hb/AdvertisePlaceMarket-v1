using DataAccess.Services.AdvertisementCategory;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/categories")]
public class AdvertisementCategoryController(IAdvertisementCategoryService service) : ControllerBase
{
    [HttpGet("menu")]
    public async Task<IActionResult> Menu() => Ok(await service.GetMenuAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var result = await service.GetDetailsAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-slug/{slug}")]
    public async Task<IActionResult> BySlug(string slug)
    {
        var result = await service.GetBySlugAsync(slug);
        return result is null ? NotFound() : Ok(result);
    }
}
