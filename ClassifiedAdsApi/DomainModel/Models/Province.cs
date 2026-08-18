namespace DomainModel.Models;

public class Province
{
    public int ProvinceID { get; set; }
    public string ProvinceName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    public string? Slug { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OpenGraphImageUrl { get; set; }
    public bool? IsIndexable { get; set; }
    public bool? IsFollow { get; set; }

    public ICollection<City> Cities { get; set; } = new List<City>();
    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
