using Api.Extensions;
using DataAccess.Services.Account;
using DataAccess.Services.Advertisement;
using DataAccess.Services.HeroBanner;
using DataAccess.Services.SiteSetting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/file-manager")]
public class FileManagerController(
    Api.FileManager.IFileManager fileManager,
    IAdvertisementService advertisementService,
    IAccountService accountService,
    ISiteSettingService siteSettingService,
    IHeroBannerService heroBannerService) : ControllerBase
{
    [Authorize(Roles = "Customer,Admin")]
    [HttpPost("advertisements/{advertisementId:long}/images")]
    public async Task<IActionResult> UploadAdvertisementImage(
        long advertisementId,
        IFormFile file,
        [FromForm] string? title,
        [FromForm] string? altText,
        [FromForm] bool isMain = false)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var isAdmin = User.IsInRole("Admin");
        if (!await advertisementService.CanManageAsync(userId, advertisementId, isAdmin)) return Forbid();

        var saved = await fileManager.SaveAdvertisementImageAsync(file);
        if (!saved.Success || saved.FilePath is null || saved.ThumbnailPath is null || saved.FileName is null)
            return BadRequest(saved);

        var result = await advertisementService.AddImageMetadataAsync(
            userId, advertisementId, saved.FileName, saved.FilePath, saved.ThumbnailPath,
            title, altText, isMain, isAdmin);

        if (!result.Success)
        {
            await fileManager.DeleteFileAsync(saved.FilePath);
            await fileManager.DeleteFileAsync(saved.ThumbnailPath);
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Customer,Admin")]
    [HttpDelete("advertisements/{advertisementId:long}/images/{imageId:long}")]
    public async Task<IActionResult> DeleteAdvertisementImage(long advertisementId, long imageId)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var isAdmin = User.IsInRole("Admin");
        var image = await advertisementService.GetImageForManagementAsync(userId, advertisementId, imageId, isAdmin);
        if (image is null) return NotFound();

        var result = await advertisementService.DeleteImageMetadataAsync(userId, advertisementId, imageId, isAdmin);
        if (!result.Success) return BadRequest(result);

        var original = await fileManager.DeleteFileAsync(image.ImagePath);
        var thumb = await fileManager.DeleteFileAsync(image.ThumbnailPath);
        if (!original || !thumb)
            return StatusCode(500, new Framework.Common.OperationResult("حذف تصویر").ToFailed("اطلاعات تصویر حذف شد اما حذف فایل فیزیکی کامل انجام نشد."));

        return Ok(result);
    }

    [Authorize(Roles = "Customer,Admin")]
    [HttpPatch("advertisements/{advertisementId:long}/images/{imageId:long}/main")]
    public async Task<IActionResult> SetMainImage(long advertisementId, long imageId)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var result = await advertisementService.SetMainImageAsync(userId, advertisementId, imageId, User.IsInRole("Admin"));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("customer/avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var oldPath = await accountService.GetAvatarPathAsync(userId);
        var saved = await fileManager.SaveAvatarAsync(file);
        if (!saved.Success || saved.FilePath is null) return BadRequest(saved);

        var result = await accountService.UpdateAvatarPathAsync(userId, saved.FilePath);
        if (!result.Success)
        {
            await fileManager.DeleteFileAsync(saved.FilePath);
            return BadRequest(result);
        }

        await fileManager.DeleteFileAsync(oldPath);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("site/logo")]
    public async Task<IActionResult> UploadLogo(IFormFile file)
    {
        var old = await siteSettingService.GetLogoPathAsync();
        var saved = await fileManager.SaveSiteImageAsync(file);
        if (!saved.Success || saved.FilePath is null) return BadRequest(saved);
        var result = await siteSettingService.UpdateLogoAsync(saved.FilePath);
        if (!result.Success) { await fileManager.DeleteFileAsync(saved.FilePath); return BadRequest(result); }
        await fileManager.DeleteFileAsync(old);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("site/favicon")]
    public async Task<IActionResult> UploadFavicon(IFormFile file)
    {
        var old = await siteSettingService.GetFaviconPathAsync();
        var saved = await fileManager.SaveSiteImageAsync(file);
        if (!saved.Success || saved.FilePath is null) return BadRequest(saved);
        var result = await siteSettingService.UpdateFaviconAsync(saved.FilePath);
        if (!result.Success) { await fileManager.DeleteFileAsync(saved.FilePath); return BadRequest(result); }
        await fileManager.DeleteFileAsync(old);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("hero-banners/{id:long}/image")]
    public async Task<IActionResult> UploadHeroBanner(long id, IFormFile file)
    {
        var old = await heroBannerService.GetImagePathAsync(id);
        if (old is null && !await HeroExists(id)) return NotFound();
        var saved = await fileManager.SaveHeroBannerAsync(file);
        if (!saved.Success || saved.FilePath is null) return BadRequest(saved);
        var result = await heroBannerService.UpdateImageAsync(id, saved.FilePath);
        if (!result.Success) { await fileManager.DeleteFileAsync(saved.FilePath); return BadRequest(result); }
        await fileManager.DeleteFileAsync(old);
        return Ok(result);
    }

    private async Task<bool> HeroExists(long id)
    {
        var search = await heroBannerService.SearchAsync(new DomainModel.ViewModels.HeroBanner.HeroBannerSearchModel { PageIndex = 1, PageSize = 100 });
        return search.Items.Any(x => x.HeroBannerID == id);
    }
}
