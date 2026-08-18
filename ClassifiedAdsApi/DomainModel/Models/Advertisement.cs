namespace DomainModel.Models;

public class Advertisement
{
    public long AdvertisementID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;

    public int AdvertisementCategoryID { get; set; }
    public int ProvinceID { get; set; }
    public int CityID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public long UserMembershipID { get; set; }
    public int AdvertisementStatusID { get; set; }

    public bool IsImmediate { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpireDate { get; set; }

    public string? RejectionReason { get; set; }
    public string? ReviewedByUserID { get; set; }
    public DateTime? ReviewedDate { get; set; }

    public string? Slug { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OpenGraphImageUrl { get; set; }
    public bool? IsIndexable { get; set; }
    public bool? IsFollow { get; set; }

    public AdvertisementCategory AdvertisementCategory { get; set; } = null!;
    public Province Province { get; set; } = null!;
    public City City { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public ApplicationUser? ReviewedByUser { get; set; }
    public UserMembership UserMembership { get; set; } = null!;
    public AdvertisementStatus AdvertisementStatus { get; set; } = null!;
    public ICollection<AdvertisementImage> Images { get; set; } = new List<AdvertisementImage>();
    public ICollection<FavoriteAdvertisement> Favorites { get; set; } = new List<FavoriteAdvertisement>();
}
