using DomainModel.Context;
using DomainModel.Models;
using Framework.Common.Constants;
using Microsoft.EntityFrameworkCore;
namespace DataAccess.Repositories.Advertisement;
public class AdvertisementRepository(ApplicationDbContext context) : IAdvertisementRepository
{
    public IQueryable<DomainModel.Models.Advertisement> Query(bool tracking = false) => tracking ? context.Advertisements : context.Advertisements.AsNoTracking();
    public IQueryable<FavoriteAdvertisement> FavoriteQuery(bool tracking = false) => tracking ? context.FavoriteAdvertisements : context.FavoriteAdvertisements.AsNoTracking();
    public Task<DomainModel.Models.Advertisement?> GetByIdAsync(long id, bool tracking = true) => (tracking ? context.Advertisements : context.Advertisements.AsNoTracking()).FirstOrDefaultAsync(x=>x.AdvertisementID==id);
    public Task<DomainModel.Models.Advertisement?> GetOwnedByIdAsync(long id, string userId, bool tracking = true) => (tracking ? context.Advertisements : context.Advertisements.AsNoTracking()).FirstOrDefaultAsync(x=>x.AdvertisementID==id && x.UserID==userId);
    public Task<DomainModel.Models.AdvertisementCategory?> GetCategoryAsync(int categoryId) => context.AdvertisementCategories.FirstOrDefaultAsync(x=>x.AdvertisementCategoryID==categoryId);
    public Task<DomainModel.Models.City?> GetCityAsync(int cityId) => context.Cities.AsNoTracking().FirstOrDefaultAsync(x=>x.CityID==cityId);
    public Task<DomainModel.Models.Province?> GetProvinceAsync(int provinceId) => context.Provinces.AsNoTracking().FirstOrDefaultAsync(x=>x.ProvinceID==provinceId);
    public Task<AdvertisementStatus?> GetStatusByCodeAsync(string code) => context.AdvertisementStatuses.AsNoTracking().FirstOrDefaultAsync(x=>x.Code==code);
    public Task<UserMembership?> GetActiveMembershipAsync(string userId) => context.UserMemberships.Include(x=>x.MembershipPlan).Where(x=>x.UserID==userId && x.IsActive).OrderByDescending(x=>x.StartDate).FirstOrDefaultAsync();
    public Task<int> CountAdvertisementsForMembershipAsync(long userMembershipId) => context.Advertisements.CountAsync(x=>x.UserMembershipID==userMembershipId);
    public Task AddAsync(DomainModel.Models.Advertisement entity) => context.Advertisements.AddAsync(entity).AsTask();
    public void Remove(DomainModel.Models.Advertisement entity) => context.Advertisements.Remove(entity);
    public Task<List<AdvertisementImage>> GetImagesAsync(long advertisementId) => context.AdvertisementImages.Where(x=>x.AdvertisementID==advertisementId).OrderBy(x=>x.DisplayOrder).ToListAsync();
    public Task<AdvertisementImage?> GetImageAsync(long advertisementId,long imageId) => context.AdvertisementImages.FirstOrDefaultAsync(x=>x.AdvertisementID==advertisementId && x.AdvertisementImageID==imageId);
    public Task AddImageAsync(AdvertisementImage image) => context.AdvertisementImages.AddAsync(image).AsTask();
    public void RemoveImage(AdvertisementImage image) => context.AdvertisementImages.Remove(image);
    public Task<FavoriteAdvertisement?> GetFavoriteAsync(string userId,long advertisementId) => context.FavoriteAdvertisements.FirstOrDefaultAsync(x=>x.UserID==userId && x.AdvertisementID==advertisementId);
    public Task AddFavoriteAsync(FavoriteAdvertisement favorite) => context.FavoriteAdvertisements.AddAsync(favorite).AsTask();
    public void RemoveFavorite(FavoriteAdvertisement favorite) => context.FavoriteAdvertisements.Remove(favorite);
    public async Task RefreshCategoryCountsAsync(int childCategoryId)
    {
        var child = await context.AdvertisementCategories.FirstOrDefaultAsync(x=>x.AdvertisementCategoryID==childCategoryId);
        if(child is null) return;
        var publishedId = await context.AdvertisementStatuses.Where(x=>x.Code==AdvertisementStatusCodes.Published).Select(x=>x.AdvertisementStatusID).FirstOrDefaultAsync();
        child.AdvertisementCount = await context.Advertisements.CountAsync(x=>x.AdvertisementCategoryID==childCategoryId && x.AdvertisementStatusID==publishedId);
        if(child.ParentID.HasValue)
        {
            var parent = await context.AdvertisementCategories.FirstOrDefaultAsync(x=>x.AdvertisementCategoryID==child.ParentID.Value);
            if(parent is not null)
            {
                var childIds = await context.AdvertisementCategories.Where(x=>x.ParentID==parent.AdvertisementCategoryID).Select(x=>x.AdvertisementCategoryID).ToListAsync();
                parent.AdvertisementCount = await context.Advertisements.CountAsync(x=>childIds.Contains(x.AdvertisementCategoryID) && x.AdvertisementStatusID==publishedId);
            }
        }
    }
    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}
