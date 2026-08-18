using DomainModel.ViewModels.Advertisement; using DomainModel.ViewModels.AdvertisementCategory; using DomainModel.ViewModels.HeroBanner; using DomainModel.ViewModels.SiteSetting;
namespace DomainModel.ViewModels.Common;
public class HomePageModel
{
    public PublicSiteSettingModel SiteSetting { get; set; } = new();
    public List<HeroBannerListItem> HeroBanners { get; set; }=new();
    public List<AdvertisementCategoryMenuItem> Categories { get; set; }=new();
    public List<AdvertisementListItem> FeaturedAdvertisements { get; set; }=new();
    public List<AdvertisementListItem> LatestAdvertisements { get; set; }=new();
}
