using DomainModel.ViewModels.AdvertisementCategory; using Framework.Common.Seo;
namespace DataAccess.Repositories.AdvertisementCategory;
public static class AdvertisementCategoryMapper
{
 public static DomainModel.Models.AdvertisementCategory ToEntity(AdvertisementCategoryAddEditModel m,int depth,int sortOrder){return new(){CategoryName=m.CategoryName.Trim(),ParentID=m.ParentID,Depth=depth,SortOrder=sortOrder,Lineage="",Slug=string.IsNullOrWhiteSpace(m.Slug)?SeoHelper.ToSlug(m.CategoryName):SeoHelper.ToSlug(m.Slug),SeoTitle=m.SeoTitle,SeoDescription=m.SeoDescription,SeoKeywords=m.SeoKeywords,CanonicalUrl=m.CanonicalUrl,OpenGraphImageUrl=m.OpenGraphImageUrl,IsIndexable=m.IsIndexable,IsFollow=m.IsFollow,IsActive=m.IsActive,Icon=m.Icon,CreateDate=DateTime.Now};}
 public static void MapForUpdate(AdvertisementCategoryAddEditModel m,DomainModel.Models.AdvertisementCategory e,int depth,int sortOrder){e.CategoryName=m.CategoryName.Trim();e.ParentID=m.ParentID;e.Depth=depth;e.SortOrder=sortOrder;e.Slug=string.IsNullOrWhiteSpace(m.Slug)?e.Slug:SeoHelper.ToSlug(m.Slug);e.SeoTitle=m.SeoTitle;e.SeoDescription=m.SeoDescription;e.SeoKeywords=m.SeoKeywords;e.CanonicalUrl=m.CanonicalUrl;e.OpenGraphImageUrl=m.OpenGraphImageUrl;e.IsIndexable=m.IsIndexable;e.IsFollow=m.IsFollow;e.IsActive=m.IsActive;e.Icon=m.Icon;e.UpdateDate=DateTime.Now;}
}
