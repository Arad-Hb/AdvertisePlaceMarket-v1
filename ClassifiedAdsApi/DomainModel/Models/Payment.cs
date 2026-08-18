namespace DomainModel.Models;

public class Payment
{
    public long PaymentID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public int MembershipPlanID { get; set; }
    public decimal Amount { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime? PaidDate { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public MembershipPlan MembershipPlan { get; set; } = null!;
}
