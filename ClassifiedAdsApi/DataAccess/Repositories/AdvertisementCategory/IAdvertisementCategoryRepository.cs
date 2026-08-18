using DomainModel.Models;
namespace DataAccess.Repositories.AdvertisementCategory;
public interface IAdvertisementCategoryRepository
{
 IQueryable<DomainModel.Models.AdvertisementCategory> Query(bool tracking=false);
 Task<DomainModel.Models.AdvertisementCategory?> GetByIdAsync(int id,bool tracking=true);
 Task AddAsync(DomainModel.Models.AdvertisementCategory entity);
 void Remove(DomainModel.Models.AdvertisementCategory entity);
 Task<bool> HasChildrenAsync(int id);
 Task<bool> HasAdvertisementsAsync(int id);
 Task<int> SaveChangesAsync();
}
