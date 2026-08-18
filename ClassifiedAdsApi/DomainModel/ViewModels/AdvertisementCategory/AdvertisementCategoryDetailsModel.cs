namespace DomainModel.ViewModels.AdvertisementCategory;
public class AdvertisementCategoryDetailsModel : AdvertisementCategoryListItem
{
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OpenGraphImageUrl { get; set; }
    public bool? IsIndexable { get; set; }
    public bool? IsFollow { get; set; }
}
