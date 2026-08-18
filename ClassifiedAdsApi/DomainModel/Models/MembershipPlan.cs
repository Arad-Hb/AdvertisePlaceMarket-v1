namespace DomainModel.Models;

public class MembershipPlan
{
    public int MembershipPlanID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationDays { get; set; }
    public int AdvertisementLimit { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; }
    public ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
