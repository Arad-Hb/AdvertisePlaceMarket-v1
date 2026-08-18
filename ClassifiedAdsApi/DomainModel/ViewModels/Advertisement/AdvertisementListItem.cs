namespace DomainModel.ViewModels.Advertisement;
public class AdvertisementListItem
{
    public long AdvertisementID { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsImmediate { get; set; }
    public bool IsFeatured { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategorySlug { get; set; }
    public string ProvinceName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateDatePersian { get; set; } = string.Empty;
    public string? Slug { get; set; }
}
