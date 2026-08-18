using Api.Extensions;
using DataAccess.Services.Advertisement;
using DataAccess.Services.Membership;
using DataAccess.Services.Payment;
using DomainModel.ViewModels.Advertisement;
using DomainModel.ViewModels.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize(Roles = "Customer")]
[Route("api/customer")]
public class CustomerController(
    IAdvertisementService advertisementService,
    IMembershipService membershipService,
    IPaymentService paymentService,
    Api.FileManager.IFileManager fileManager) : ControllerBase
{
    private string? CurrentUserId => User.GetUserId();

    [HttpGet("advertisements")]
    public async Task<IActionResult> Advertisements([FromQuery] AdvertisementSearchModel model)
    {
        if (CurrentUserId is null) return Unauthorized();
        return Ok(await advertisementService.SearchCustomerAsync(CurrentUserId, model));
    }

    [HttpGet("advertisements/{id:long}")]
    public async Task<IActionResult> Advertisement(long id)
    {
        if (CurrentUserId is null) return Unauthorized();
        var result = await advertisementService.GetCustomerDetailsAsync(CurrentUserId, id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("advertisements")]
    public async Task<IActionResult> CreateAdvertisement(AdvertisementAddModel model)
    {
        if (CurrentUserId is null) return Unauthorized();
        var result = await advertisementService.AddAsync(CurrentUserId, model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("advertisements/{id:long}")]
    public async Task<IActionResult> UpdateAdvertisement(long id, AdvertisementEditModel model)
    {
        if (CurrentUserId is null) return Unauthorized();
        var result = await advertisementService.UpdateAsync(CurrentUserId, id, model);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("advertisements/{id:long}")]
    public async Task<IActionResult> DeleteAdvertisement(long id)
    {
        if (CurrentUserId is null) return Unauthorized();
        var images = await advertisementService.GetImagesForManagementAsync(CurrentUserId, id);
        if (images is null) return NotFound();

        var result = await advertisementService.DeleteAsync(CurrentUserId, id);
        if (!result.Success) return BadRequest(result);

        foreach (var image in images)
        {
            var originalDeleted = await fileManager.DeleteFileAsync(image.ImagePath);
            var thumbnailDeleted = await fileManager.DeleteFileAsync(image.ThumbnailPath);
            if (!originalDeleted || !thumbnailDeleted)
                return StatusCode(500, new Framework.Common.OperationResult("حذف فایل‌های آگهی").ToFailed("آگهی حذف شد اما حذف یکی از فایل‌های فیزیکی کامل انجام نشد."));
        }

        return Ok(result);
    }

    [HttpPost("advertisements/{id:long}/submit")]
    public async Task<IActionResult> SubmitAdvertisement(long id)
    {
        if (CurrentUserId is null) return Unauthorized();
        var result = await advertisementService.SubmitAsync(CurrentUserId, id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> Favorites([FromQuery] AdvertisementSearchModel model)
    {
        if (CurrentUserId is null) return Unauthorized();
        return Ok(await advertisementService.GetFavoritesAsync(CurrentUserId, model));
    }

    [HttpPost("favorites/{advertisementId:long}")]
    public async Task<IActionResult> AddFavorite(long advertisementId)
    {
        if (CurrentUserId is null) return Unauthorized();
        var result = await advertisementService.AddFavoriteAsync(CurrentUserId, advertisementId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("favorites/{advertisementId:long}")]
    public async Task<IActionResult> RemoveFavorite(long advertisementId)
    {
        if (CurrentUserId is null) return Unauthorized();
        var result = await advertisementService.RemoveFavoriteAsync(CurrentUserId, advertisementId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("membership")]
    public async Task<IActionResult> Membership()
    {
        if (CurrentUserId is null) return Unauthorized();
        return Ok(await membershipService.GetCurrentAsync(CurrentUserId));
    }

    [HttpPost("membership/purchase/{membershipPlanId:int}")]
    public async Task<IActionResult> PurchaseMembership(int membershipPlanId)
    {
        if (CurrentUserId is null) return Unauthorized();
        var result = await membershipService.PurchaseAsync(CurrentUserId, membershipPlanId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payments")]
    public async Task<IActionResult> Payments([FromQuery] PaymentSearchModel model)
    {
        if (CurrentUserId is null) return Unauthorized();
        return Ok(await paymentService.GetCustomerPaymentsAsync(CurrentUserId, model));
    }
}
