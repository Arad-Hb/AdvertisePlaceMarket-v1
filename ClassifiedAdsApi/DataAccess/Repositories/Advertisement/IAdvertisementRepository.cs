using DomainModel.Models;
namespace DataAccess.Repositories.Advertisement;
public interface IAdvertisementRepository
{
    IQueryable<DomainModel.Models.Advertisement> Query(bool tracking = false);
    IQueryable<FavoriteAdvertisement> FavoriteQuery(bool tracking = false);
    Task<DomainModel.Models.Advertisement?> GetByIdAsync(long id, bool tracking = true);
    Task<DomainModel.Models.Advertisement?> GetOwnedByIdAsync(long id, string userId, bool tracking = true);
    Task<DomainModel.Models.AdvertisementCategory?> GetCategoryAsync(int categoryId);
    Task<DomainModel.Models.City?> GetCityAsync(int cityId);
    Task<DomainModel.Models.Province?> GetProvinceAsync(int provinceId);
    Task<AdvertisementStatus?> GetStatusByCodeAsync(string code);
    Task<UserMembership?> GetActiveMembershipAsync(string userId);
    Task<int> CountAdvertisementsForMembershipAsync(long userMembershipId);
    Task AddAsync(DomainModel.Models.Advertisement entity);
    void Remove(DomainModel.Models.Advertisement entity);
    Task<List<AdvertisementImage>> GetImagesAsync(long advertisementId);
    Task<AdvertisementImage?> GetImageAsync(long advertisementId, long imageId);
    Task AddImageAsync(AdvertisementImage image);
    void RemoveImage(AdvertisementImage image);
    Task<FavoriteAdvertisement?> GetFavoriteAsync(string userId, long advertisementId);
    Task AddFavoriteAsync(FavoriteAdvertisement favorite);
    void RemoveFavorite(FavoriteAdvertisement favorite);
    Task RefreshCategoryCountsAsync(int childCategoryId);
    Task<int> SaveChangesAsync();
}
