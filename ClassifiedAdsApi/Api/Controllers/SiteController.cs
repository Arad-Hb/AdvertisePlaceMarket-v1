using DataAccess.Services.Advertisement;
using DataAccess.Services.AdvertisementCategory;
using DataAccess.Services.HeroBanner;
using DataAccess.Services.SiteSetting;
using DomainModel.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/site")]
public class SiteController(
    ISiteSettingService settingService,
    IHeroBannerService heroService,
    IAdvertisementCategoryService categoryService,
    IAdvertisementService advertisementService) : ControllerBase
{
    [HttpGet("settings")]
    public async Task<IActionResult> Settings() => Ok(await settingService.GetPublicAsync());

    [HttpGet("home")]
    public async Task<IActionResult> Home()
    {
        var result = new HomePageModel
        {
            SiteSetting = await settingService.GetPublicAsync(),
            HeroBanners = await heroService.GetActiveAsync(),
            Categories = await categoryService.GetMenuAsync(),
            FeaturedAdvertisements = await advertisementService.GetFeaturedAsync(),
            LatestAdvertisements = await advertisementService.GetLatestAsync()
        };
        return Ok(result);
    }
}
