namespace DomainModel.ViewModels.Payment;
public class PaymentListItem
{
    public long PaymentID { get; set; }
    public string UserID { get; set; }=string.Empty;
    public string CustomerName { get; set; }=string.Empty;
    public string MobileNumber { get; set; }=string.Empty;
    public int MembershipPlanID { get; set; }
    public string MembershipPlanTitle { get; set; }=string.Empty;
    public decimal Amount { get; set; }
    public string TrackingCode { get; set; }=string.Empty;
    public bool IsPaid { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateDatePersian { get; set; }=string.Empty;
    public DateTime? PaidDate { get; set; }
    public string? PaidDatePersian { get; set; }
}
