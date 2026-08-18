using DomainModel.ViewModels.Advertisement; using Framework.Common;
namespace DataAccess.Services.Advertisement;
public interface IAdvertisementService
{
 Task<AdvertisementListComplexModel> SearchPublicAsync(AdvertisementSearchModel model);
 Task<AdvertisementDetailsModel?> GetPublicDetailsAsync(long id,string? currentUserId=null);
 Task<AdvertisementDetailsModel?> GetPublicDetailsBySlugAsync(string slug,string? currentUserId=null);
 Task<List<AdvertisementListItem>> GetFeaturedAsync(int count=8);
 Task<List<AdvertisementListItem>> GetLatestAsync(int count=12);
 Task<CustomerAdvertisementListComplexModel> SearchCustomerAsync(string userId,AdvertisementSearchModel model);
 Task<CustomerAdvertisementDetailsModel?> GetCustomerDetailsAsync(string userId,long id);
 Task<OperationResult> AddAsync(string userId,AdvertisementAddModel model);
 Task<OperationResult> UpdateAsync(string userId,long id,AdvertisementEditModel model);
 Task<OperationResult> DeleteAsync(string userId,long id);
 Task<OperationResult> SubmitAsync(string userId,long id);
 Task<AdminAdvertisementListComplexModel> SearchAdminAsync(AdminAdvertisementSearchModel model);
 Task<CustomerAdvertisementDetailsModel?> GetAdminDetailsAsync(long id);
 Task<OperationResult> ApproveAsync(string adminUserId,long id);
 Task<OperationResult> RejectAsync(string adminUserId,long id,AdvertisementRejectModel model);
 Task<OperationResult> DisableAsync(string adminUserId,long id);
 Task<OperationResult> SetFeaturedAsync(long id,bool featured);
 Task<AdvertisementListComplexModel> GetFavoritesAsync(string userId,AdvertisementSearchModel model);
 Task<OperationResult> AddFavoriteAsync(string userId,long advertisementId);
 Task<OperationResult> RemoveFavoriteAsync(string userId,long advertisementId);
 Task<bool> CanManageAsync(string userId,long advertisementId,bool isAdmin=false);
 Task<List<AdvertisementImageModel>?> GetImagesForManagementAsync(string userId,long advertisementId,bool isAdmin=false);
 Task<AdvertisementImageModel?> GetImageForManagementAsync(string userId,long advertisementId,long imageId,bool isAdmin=false);
 Task<OperationResult> AddImageMetadataAsync(string userId,long advertisementId,string imageName,string imagePath,string thumbnailPath,string? title,string? altText,bool isMain,bool isAdmin=false);
 Task<OperationResult> DeleteImageMetadataAsync(string userId,long advertisementId,long imageId,bool isAdmin=false);
 Task<OperationResult> SetMainImageAsync(string userId,long advertisementId,long imageId,bool isAdmin=false);
}