namespace DomainModel.ViewModels.AdvertisementCategory;
public class AdvertisementCategoryMenuItem
{
    public int AdvertisementCategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Icon { get; set; }
    public int AdvertisementCount { get; set; }
    public List<AdvertisementCategoryMenuItem> Children { get; set; } = new();
}
