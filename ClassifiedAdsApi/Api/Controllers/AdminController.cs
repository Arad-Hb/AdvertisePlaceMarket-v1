using Api.Extensions;
using DataAccess.Services.Admin;
using DataAccess.Services.Advertisement;
using DataAccess.Services.AdvertisementCategory;
using DataAccess.Services.City;
using DataAccess.Services.HeroBanner;
using DataAccess.Services.Membership;
using DataAccess.Services.Payment;
using DataAccess.Services.Province;
using DataAccess.Services.SiteSetting;
using DomainModel.ViewModels.Admin;
using DomainModel.ViewModels.Advertisement;
using DomainModel.ViewModels.AdvertisementCategory;
using DomainModel.ViewModels.City;
using DomainModel.ViewModels.HeroBanner;
using DomainModel.ViewModels.Membership;
using DomainModel.ViewModels.Payment;
using DomainModel.ViewModels.Province;
using DomainModel.ViewModels.SiteSetting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin")]
public class AdminController(
    IAdminService adminService,
    IAdvertisementService advertisementService,
    IAdvertisementCategoryService categoryService,
    IProvinceService provinceService,
    ICityService cityService,
    IMembershipService membershipService,
    IPaymentService paymentService,
    ISiteSettingService siteSettingService,
    IHeroBannerService heroBannerService,
    Api.FileManager.IFileManager fileManager) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard() => Ok(await adminService.GetDashboardAsync());

    [HttpGet("advertisements")]
    public async Task<IActionResult> Advertisements([FromQuery] AdminAdvertisementSearchModel model)
        => Ok(await advertisementService.SearchAdminAsync(model));

    [HttpGet("advertisements/{id:long}")]
    public async Task<IActionResult> Advertisement(long id)
    {
        var result = await advertisementService.GetAdminDetailsAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("advertisements/{id:long}/approve")]
    public async Task<IActionResult> Approve(long id)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var result = await advertisementService.ApproveAsync(userId, id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("advertisements/{id:long}/reject")]
    public async Task<IActionResult> Reject(long id, AdvertisementRejectModel model)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var result = await advertisementService.RejectAsync(userId, id, model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("advertisements/{id:long}/disable")]
    public async Task<IActionResult> Disable(long id)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var result = await advertisementService.DisableAsync(userId, id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("advertisements/{id:long}/feature")]
    public async Task<IActionResult> Feature(long id)
    {
        var result = await advertisementService.SetFeaturedAsync(id, true);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("advertisements/{id:long}/unfeature")]
    public async Task<IActionResult> Unfeature(long id)
    {
        var result = await advertisementService.SetFeaturedAsync(id, false);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("customers")]
    public async Task<IActionResult> Customers([FromQuery] CustomerSearchModel model)
        => Ok(await adminService.SearchCustomersAsync(model));

    [HttpGet("customers/{id}")]
    public async Task<IActionResult> Customer(string id)
    {
        var result = await adminService.GetCustomerAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("customers")]
    public async Task<IActionResult> AddCustomer(CustomerAddEditModel model)
    {
        var result = await adminService.AddCustomerAsync(model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("customers/{id}")]
    public async Task<IActionResult> UpdateCustomer(string id, CustomerAddEditModel model)
    {
        var result = await adminService.UpdateCustomerAsync(id, model);
        return result.Success ? Ok(result) : (result.Message == "مشتری پیدا نشد." ? NotFound(result) : BadRequest(result));
    }

    [HttpDelete("customers/{id}")]
    public async Task<IActionResult> DeleteCustomer(string id)
    {
        var result = await adminService.DeleteCustomerAsync(id);
        return result.Success ? Ok(result) : (result.Message == "مشتری پیدا نشد." ? NotFound(result) : BadRequest(result));
    }

    [HttpPatch("customers/{id}/activate")]
    public async Task<IActionResult> ActivateCustomer(string id)
    {
        var result = await adminService.SetCustomerActiveAsync(id, true);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPatch("customers/{id}/deactivate")]
    public async Task<IActionResult> DeactivateCustomer(string id)
    {
        var result = await adminService.SetCustomerActiveAsync(id, false);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> Categories([FromQuery] AdvertisementCategorySearchModel model)
        => Ok(await categoryService.SearchAsync(model));

    [HttpPost("categories")]
    public async Task<IActionResult> AddCategory(AdvertisementCategoryAddEditModel model)
    {
        var result = await categoryService.AddAsync(model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("categories/{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, AdvertisementCategoryAddEditModel model)
    {
        var result = await categoryService.UpdateAsync(id, model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("categories/{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await categoryService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("provinces")]
    public async Task<IActionResult> Provinces([FromQuery] ProvinceSearchModel model)
        => Ok(await provinceService.SearchAsync(model));

    [HttpPost("provinces")]
    public async Task<IActionResult> AddProvince(ProvinceAddEditModel model)
    {
        var result = await provinceService.AddAsync(model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("provinces/{id:int}")]
    public async Task<IActionResult> UpdateProvince(int id, ProvinceAddEditModel model)
    {
        var result = await provinceService.UpdateAsync(id, model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("provinces/{id:int}")]
    public async Task<IActionResult> DeleteProvince(int id)
    {
        var result = await provinceService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("cities")]
    public async Task<IActionResult> Cities([FromQuery] CitySearchModel model)
        => Ok(await cityService.SearchAsync(model));

    [HttpPost("cities")]
    public async Task<IActionResult> AddCity(CityAddEditModel model)
    {
        var result = await cityService.AddAsync(model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("cities/{id:int}")]
    public async Task<IActionResult> UpdateCity(int id, CityAddEditModel model)
    {
        var result = await cityService.UpdateAsync(id, model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("cities/{id:int}")]
    public async Task<IActionResult> DeleteCity(int id)
    {
        var result = await cityService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("membership-plans")]
    public async Task<IActionResult> MembershipPlans([FromQuery] MembershipPlanSearchModel model)
        => Ok(await membershipService.SearchAsync(model));

    [HttpPost("membership-plans")]
    public async Task<IActionResult> AddMembershipPlan(MembershipPlanAddEditModel model)
    {
        var result = await membershipService.AddPlanAsync(model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("membership-plans/{id:int}")]
    public async Task<IActionResult> UpdateMembershipPlan(int id, MembershipPlanAddEditModel model)
    {
        var result = await membershipService.UpdatePlanAsync(id, model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("membership-plans/{id:int}")]
    public async Task<IActionResult> DeleteMembershipPlan(int id)
    {
        var result = await membershipService.DeleteOrDeactivatePlanAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payments")]
    public async Task<IActionResult> Payments([FromQuery] PaymentSearchModel model)
        => Ok(await paymentService.SearchAdminAsync(model));

    [HttpGet("site-setting")]
    public async Task<IActionResult> SiteSetting()
    {
        var result = await siteSettingService.GetAdminAsync();
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("site-setting")]
    public async Task<IActionResult> UpdateSiteSetting(SiteSettingEditModel model)
    {
        var result = await siteSettingService.UpdateAsync(model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("hero-banners")]
    public async Task<IActionResult> HeroBanners([FromQuery] HeroBannerSearchModel model)
        => Ok(await heroBannerService.SearchAsync(model));

    [HttpPost("hero-banners")]
    public async Task<IActionResult> AddHeroBanner(HeroBannerAddEditModel model)
    {
        var result = await heroBannerService.AddAsync(model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("hero-banners/{id:long}")]
    public async Task<IActionResult> UpdateHeroBanner(long id, HeroBannerAddEditModel model)
    {
        var result = await heroBannerService.UpdateAsync(id, model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("hero-banners/{id:long}")]
    public async Task<IActionResult> DeleteHeroBanner(long id)
    {
        var imagePath = await heroBannerService.GetImagePathAsync(id);
        var result = await heroBannerService.DeleteAsync(id);
        if (!result.Success) return BadRequest(result);
        await fileManager.DeleteFileAsync(imagePath);
        return Ok(result);
    }

    [HttpPatch("hero-banners/{id:long}/activate")]
    public async Task<IActionResult> ActivateHeroBanner(long id)
    {
        var result = await heroBannerService.SetActiveAsync(id, true);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("hero-banners/{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateHeroBanner(long id)
    {
        var result = await heroBannerService.SetActiveAsync(id, false);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
