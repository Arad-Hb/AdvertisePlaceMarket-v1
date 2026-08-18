namespace DomainModel.ViewModels.AdvertisementCategory;
public class AdvertisementCategoryListItem
{
    public int AdvertisementCategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentID { get; set; }
    public string? ParentName { get; set; }
    public int Depth { get; set; }
    public string Lineage { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int AdvertisementCount { get; set; }
    public string? Slug { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
}
