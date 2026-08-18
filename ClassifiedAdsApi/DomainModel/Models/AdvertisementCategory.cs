namespace DomainModel.Models;

public class AdvertisementCategory
{
    public int AdvertisementCategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentID { get; set; }
    public int Depth { get; set; }
    public string Lineage { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int AdvertisementCount { get; set; }

    public string? Slug { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OpenGraphImageUrl { get; set; }
    public bool? IsIndexable { get; set; }
    public bool? IsFollow { get; set; }

    public bool IsActive { get; set; } = true;
    public string? Icon { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; }

    public AdvertisementCategory? Parent { get; set; }
    public ICollection<AdvertisementCategory> Children { get; set; } = new List<AdvertisementCategory>();
    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
