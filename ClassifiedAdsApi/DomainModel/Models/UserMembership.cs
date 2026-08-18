namespace DomainModel.Models;

public class UserMembership
{
    public long UserMembershipID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public int MembershipPlanID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PaidAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreateDate { get; set; } = DateTime.Now;

    public ApplicationUser User { get; set; } = null!;
    public MembershipPlan MembershipPlan { get; set; } = null!;
    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
