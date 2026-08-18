using DomainModel.Models;
using DomainModel.ViewModels.Advertisement;
using Framework.Common.Seo;
namespace DataAccess.Repositories.Advertisement;
public static class AdvertisementMapper
{
    public static DomainModel.Models.Advertisement ToEntity(AdvertisementAddModel model,string userId,long membershipId,int draftStatusId)
    {
        return new DomainModel.Models.Advertisement
        {
            Title=model.Title.Trim(), Description=model.Description.Trim(), Price=model.Price, PhoneNumber=model.PhoneNumber.Trim(),
            AdvertisementCategoryID=model.AdvertisementCategoryID, ProvinceID=model.ProvinceID, CityID=model.CityID,
            UserID=userId, UserMembershipID=membershipId, AdvertisementStatusID=draftStatusId,
            IsImmediate=model.IsImmediate, IsFeatured=false, ViewCount=0, CreateDate=DateTime.Now,
            Slug=SeoHelper.ToSlug(model.Title), SeoTitle=model.SeoTitle, SeoDescription=model.SeoDescription,
            SeoKeywords=model.SeoKeywords, CanonicalUrl=model.CanonicalUrl, OpenGraphImageUrl=model.OpenGraphImageUrl,
            IsIndexable=model.IsIndexable, IsFollow=model.IsFollow
        };
    }
    public static void MapForUpdate(AdvertisementEditModel model,DomainModel.Models.Advertisement entity)
    {
        entity.Title=model.Title.Trim(); entity.Description=model.Description.Trim(); entity.Price=model.Price; entity.PhoneNumber=model.PhoneNumber.Trim();
        entity.AdvertisementCategoryID=model.AdvertisementCategoryID; entity.ProvinceID=model.ProvinceID; entity.CityID=model.CityID; entity.IsImmediate=model.IsImmediate;
        entity.UpdateDate=DateTime.Now;
        entity.SeoTitle=model.SeoTitle; entity.SeoDescription=model.SeoDescription; entity.SeoKeywords=model.SeoKeywords; entity.CanonicalUrl=model.CanonicalUrl; entity.OpenGraphImageUrl=model.OpenGraphImageUrl; entity.IsIndexable=model.IsIndexable; entity.IsFollow=model.IsFollow;
        if(string.IsNullOrWhiteSpace(entity.Slug)) entity.Slug=SeoHelper.ToSlug(model.Title);
    }
}
