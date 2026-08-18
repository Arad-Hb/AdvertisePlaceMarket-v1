using DomainModel.ViewModels.Common;
namespace DomainModel.ViewModels.Advertisement;
public class AdvertisementDetailsModel
{
    public long AdvertisementID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsImmediate { get; set; }
    public int AdvertisementCategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategorySlug { get; set; }
    public int ProvinceID { get; set; }
    public string ProvinceName { get; set; } = string.Empty;
    public int CityID { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; }
    public string CreateDatePersian { get; set; } = string.Empty;
    public DateTime? UpdateDate { get; set; }
    public string? UpdateDatePersian { get; set; }
    public DateTime? PublishDate { get; set; }
    public string? PublishDatePersian { get; set; }
    public int ViewCount { get; set; }
    public string? Slug { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OpenGraphImageUrl { get; set; }
    public bool? IsIndexable { get; set; }
    public bool? IsFollow { get; set; }
    public bool IsFavorite { get; set; }
    public List<AdvertisementImageModel> Images { get; set; } = new();
    public List<BreadcrumbItemModel> Breadcrumb { get; set; } = new();
}
