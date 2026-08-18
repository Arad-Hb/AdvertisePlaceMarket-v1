using Api.Extensions;
using DataAccess.Services.Advertisement;
using DomainModel.ViewModels.Advertisement;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/advertisements")]
public class AdvertisementController(IAdvertisementService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] AdvertisementSearchModel model)
        => Ok(await service.SearchPublicAsync(model));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Details(long id)
    {
        var result = await service.GetPublicDetailsAsync(id, User.Identity?.IsAuthenticated == true ? User.GetUserId() : null);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-slug/{slug}")]
    public async Task<IActionResult> BySlug(string slug)
    {
        var result = await service.GetPublicDetailsBySlugAsync(slug, User.Identity?.IsAuthenticated == true ? User.GetUserId() : null);
        return result is null ? NotFound() : Ok(result);
    }
}
