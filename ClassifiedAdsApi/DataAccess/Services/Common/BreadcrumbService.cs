using DataAccess.Repositories.AdvertisementCategory; using DomainModel.ViewModels.Common; using Microsoft.EntityFrameworkCore;
namespace DataAccess.Services.Common;
public class BreadcrumbService(IAdvertisementCategoryRepository repository):IBreadcrumbService
{
 public async Task<List<BreadcrumbItemModel>> BuildCategoryAsync(int categoryId)
 {
   var items=new List<BreadcrumbItemModel>{new(){Title="خانه",Url="/"}};
   var category=await repository.Query().FirstOrDefaultAsync(x=>x.AdvertisementCategoryID==categoryId);
   if(category is null) return items;
   if(category.ParentID.HasValue)
   {
      var parent=await repository.Query().FirstOrDefaultAsync(x=>x.AdvertisementCategoryID==category.ParentID.Value);
      if(parent is not null) items.Add(new(){Title=parent.CategoryName,Url=$"/advertisements.html?category={parent.AdvertisementCategoryID}"});
   }
   items.Add(new(){Title=category.CategoryName,Url=null});
   return items;
 }
 public async Task<List<BreadcrumbItemModel>> BuildAdvertisementAsync(int categoryId,string advertisementTitle)
 {
   var items=await BuildCategoryAsync(categoryId);
   if(items.Count>0 && items[^1].Url is null) items[^1].Url=$"/advertisements.html?category={categoryId}";
   items.Add(new(){Title=advertisementTitle,Url=null});
   return items;
 }
}
