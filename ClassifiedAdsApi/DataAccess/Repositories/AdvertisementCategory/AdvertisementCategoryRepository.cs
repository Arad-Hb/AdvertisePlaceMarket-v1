using DomainModel.Context; using Microsoft.EntityFrameworkCore;
namespace DataAccess.Repositories.AdvertisementCategory;
public class AdvertisementCategoryRepository(ApplicationDbContext context):IAdvertisementCategoryRepository
{
 public IQueryable<DomainModel.Models.AdvertisementCategory> Query(bool tracking=false)=>tracking?context.AdvertisementCategories:context.AdvertisementCategories.AsNoTracking();
 public Task<DomainModel.Models.AdvertisementCategory?> GetByIdAsync(int id,bool tracking=true)=>(tracking?context.AdvertisementCategories:context.AdvertisementCategories.AsNoTracking()).FirstOrDefaultAsync(x=>x.AdvertisementCategoryID==id);
 public Task AddAsync(DomainModel.Models.AdvertisementCategory entity)=>context.AdvertisementCategories.AddAsync(entity).AsTask();
 public void Remove(DomainModel.Models.AdvertisementCategory entity)=>context.AdvertisementCategories.Remove(entity);
 public Task<bool> HasChildrenAsync(int id)=>context.AdvertisementCategories.AnyAsync(x=>x.ParentID==id);
 public Task<bool> HasAdvertisementsAsync(int id)=>context.Advertisements.AnyAsync(x=>x.AdvertisementCategoryID==id);
 public Task<int> SaveChangesAsync()=>context.SaveChangesAsync();
}
