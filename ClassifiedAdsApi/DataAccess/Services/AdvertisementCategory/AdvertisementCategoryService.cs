using DataAccess.Repositories.AdvertisementCategory; using DataAccess.Services.Common; using DomainModel.ViewModels.AdvertisementCategory; using Framework.Common; using Framework.Common.Seo; using Microsoft.EntityFrameworkCore;
namespace DataAccess.Services.AdvertisementCategory;
public class AdvertisementCategoryService(IAdvertisementCategoryRepository repository,IPaginationService pagination):IAdvertisementCategoryService
{
 public async Task<List<AdvertisementCategoryMenuItem>> GetMenuAsync()
 {
   var rows=await repository.Query().Where(x=>x.IsActive).OrderBy(x=>x.SortOrder).ThenBy(x=>x.CategoryName).Select(x=>new AdvertisementCategoryListItem{AdvertisementCategoryID=x.AdvertisementCategoryID,CategoryName=x.CategoryName,ParentID=x.ParentID,Depth=x.Depth,SortOrder=x.SortOrder,AdvertisementCount=x.AdvertisementCount,Slug=x.Slug,Icon=x.Icon,IsActive=x.IsActive,Lineage=x.Lineage}).ToListAsync();
   var parents=rows.Where(x=>x.Depth==1).Select(x=>new AdvertisementCategoryMenuItem{AdvertisementCategoryID=x.AdvertisementCategoryID,CategoryName=x.CategoryName,Slug=x.Slug,Icon=x.Icon,AdvertisementCount=x.AdvertisementCount}).ToList();
   foreach(var p in parents) p.Children=rows.Where(x=>x.ParentID==p.AdvertisementCategoryID&&x.Depth==2).Select(x=>new AdvertisementCategoryMenuItem{AdvertisementCategoryID=x.AdvertisementCategoryID,CategoryName=x.CategoryName,Slug=x.Slug,Icon=x.Icon,AdvertisementCount=x.AdvertisementCount}).ToList();
   return parents;
 }
 public Task<AdvertisementCategoryDetailsModel?> GetDetailsAsync(int id)=>repository.Query().Where(x=>x.AdvertisementCategoryID==id).Select(x=>new AdvertisementCategoryDetailsModel{AdvertisementCategoryID=x.AdvertisementCategoryID,CategoryName=x.CategoryName,ParentID=x.ParentID,ParentName=x.Parent!=null?x.Parent.CategoryName:null,Depth=x.Depth,Lineage=x.Lineage,SortOrder=x.SortOrder,AdvertisementCount=x.AdvertisementCount,Slug=x.Slug,IsActive=x.IsActive,SeoTitle=x.SeoTitle,SeoDescription=x.SeoDescription,SeoKeywords=x.SeoKeywords,CanonicalUrl=x.CanonicalUrl,OpenGraphImageUrl=x.OpenGraphImageUrl,IsIndexable=x.IsIndexable,IsFollow=x.IsFollow,Icon=x.Icon}).FirstOrDefaultAsync();
 public Task<AdvertisementCategoryDetailsModel?> GetBySlugAsync(string slug)=>repository.Query().Where(x=>x.Slug==slug).Select(x=>new AdvertisementCategoryDetailsModel{AdvertisementCategoryID=x.AdvertisementCategoryID,CategoryName=x.CategoryName,ParentID=x.ParentID,ParentName=x.Parent!=null?x.Parent.CategoryName:null,Depth=x.Depth,Lineage=x.Lineage,SortOrder=x.SortOrder,AdvertisementCount=x.AdvertisementCount,Slug=x.Slug,IsActive=x.IsActive,SeoTitle=x.SeoTitle,SeoDescription=x.SeoDescription,SeoKeywords=x.SeoKeywords,CanonicalUrl=x.CanonicalUrl,OpenGraphImageUrl=x.OpenGraphImageUrl,IsIndexable=x.IsIndexable,IsFollow=x.IsFollow,Icon=x.Icon}).FirstOrDefaultAsync();
 public async Task<AdvertisementCategoryListComplexModel> SearchAsync(AdvertisementCategorySearchModel model)
 {
   var q=repository.Query(); if(!string.IsNullOrWhiteSpace(model.Keyword)) q=q.Where(x=>x.CategoryName.Contains(model.Keyword)); if(model.Depth.HasValue) q=q.Where(x=>x.Depth==model.Depth); if(model.IsActive.HasValue) q=q.Where(x=>x.IsActive==model.IsActive);
   var projected=q.OrderBy(x=>x.Depth).ThenBy(x=>x.SortOrder).ThenBy(x=>x.CategoryName).Select(x=>new AdvertisementCategoryListItem{AdvertisementCategoryID=x.AdvertisementCategoryID,CategoryName=x.CategoryName,ParentID=x.ParentID,ParentName=x.Parent!=null?x.Parent.CategoryName:null,Depth=x.Depth,Lineage=x.Lineage,SortOrder=x.SortOrder,AdvertisementCount=x.AdvertisementCount,Slug=x.Slug,IsActive=x.IsActive});
   return new(){Items=await pagination.PaginateAsync(projected,model),PageModel=model};
 }
 public async Task<OperationResult> AddAsync(AdvertisementCategoryAddEditModel model)
 {
   var result=new OperationResult("افزودن دسته‌بندی"); int depth; int sort;
   if(model.ParentID.HasValue){var parent=await repository.GetByIdAsync(model.ParentID.Value,false); if(parent is null) return result.ToFailed("دسته‌بندی والد پیدا نشد."); if(parent.Depth!=1) return result.ToFailed("دسته‌بندی والد باید از سطح اول باشد."); depth=2; sort=model.SortOrder??2;} else {depth=1; sort=model.SortOrder??1;}
   var slug=string.IsNullOrWhiteSpace(model.Slug)?SeoHelper.ToSlug(model.CategoryName):SeoHelper.ToSlug(model.Slug); if(slug is not null && await repository.Query().AnyAsync(x=>x.Slug==slug)) slug=$"{slug}-{Guid.NewGuid().ToString("N")[..6]}";
   var adjusted=new AdvertisementCategoryAddEditModel{CategoryName=model.CategoryName,ParentID=model.ParentID,SortOrder=sort,Slug=slug,SeoTitle=model.SeoTitle,SeoDescription=model.SeoDescription,SeoKeywords=model.SeoKeywords,CanonicalUrl=model.CanonicalUrl,OpenGraphImageUrl=model.OpenGraphImageUrl,IsIndexable=model.IsIndexable,IsFollow=model.IsFollow,IsActive=model.IsActive,Icon=model.Icon};
   var entity=AdvertisementCategoryMapper.ToEntity(adjusted,depth,sort); await repository.AddAsync(entity); await repository.SaveChangesAsync(); entity.Lineage=depth==1?$"/{entity.AdvertisementCategoryID}/":$"/{model.ParentID}/{entity.AdvertisementCategoryID}/"; await repository.SaveChangesAsync(); return result.ToSuccess("دسته‌بندی با موفقیت اضافه شد.",entity.AdvertisementCategoryID);
 }
 public async Task<OperationResult> UpdateAsync(int id,AdvertisementCategoryAddEditModel model)
 {
   var result=new OperationResult("ویرایش دسته‌بندی"); var entity=await repository.GetByIdAsync(id); if(entity is null) return result.ToFailed("دسته‌بندی پیدا نشد.");
   int depth; int sort;
   if(model.ParentID.HasValue){ if(model.ParentID==id) return result.ToFailed("دسته‌بندی نمی‌تواند والد خودش باشد."); if(await repository.HasChildrenAsync(id)) return result.ToFailed("دسته‌بندی دارای زیرمجموعه را نمی‌توان به سطح دوم منتقل کرد."); var parent=await repository.GetByIdAsync(model.ParentID.Value,false); if(parent is null||parent.Depth!=1) return result.ToFailed("دسته‌بندی والد معتبر نیست."); depth=2; sort=model.SortOrder??2;} else {depth=1; sort=model.SortOrder??1;}
   AdvertisementCategoryMapper.MapForUpdate(model,entity,depth,sort); entity.Lineage=depth==1?$"/{id}/":$"/{model.ParentID}/{id}/"; await repository.SaveChangesAsync(); return result.ToSuccess("دسته‌بندی با موفقیت ویرایش شد.",id);
 }
 public async Task<OperationResult> DeleteAsync(int id)
 {
   var result=new OperationResult("حذف دسته‌بندی"); var entity=await repository.GetByIdAsync(id); if(entity is null) return result.ToFailed("دسته‌بندی پیدا نشد.",id); if(await repository.HasChildrenAsync(id)) return result.ToFailed("ابتدا زیرمجموعه‌های این دسته‌بندی را حذف یا غیرفعال کنید.",id); if(await repository.HasAdvertisementsAsync(id)) return result.ToFailed("این دسته‌بندی دارای آگهی است و قابل حذف نیست.",id); repository.Remove(entity); await repository.SaveChangesAsync(); return result.ToSuccess("دسته‌بندی حذف شد.",id);
 }
}
