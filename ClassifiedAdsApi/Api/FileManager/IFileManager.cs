using Microsoft.AspNetCore.Http;

namespace Api.FileManager;

public interface IFileManager
{
    Task<FileManagerResult> SaveAdvertisementImageAsync(IFormFile file);
    Task<FileManagerResult> SaveAvatarAsync(IFormFile file);
    Task<FileManagerResult> SaveSiteImageAsync(IFormFile file);
    Task<FileManagerResult> SaveHeroBannerAsync(IFormFile file);
    Task<bool> DeleteFileAsync(string? webPath);
}
