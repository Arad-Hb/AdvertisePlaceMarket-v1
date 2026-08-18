namespace DomainModel.ViewModels.Membership;
public class CurrentMembershipModel
{
    public long UserMembershipID { get; set; }
    public int MembershipPlanID { get; set; }
    public string MembershipPlanTitle { get; set; }=string.Empty;
    public DateTime StartDate { get; set; }
    public string StartDatePersian { get; set; }=string.Empty;
    public DateTime EndDate { get; set; }
    public string EndDatePersian { get; set; }=string.Empty;
    public int AdvertisementLimit { get; set; }
    public int CurrentAdvertisementCount { get; set; }
    public int RemainingAdvertisements { get; set; }
    public bool IsActive { get; set; }
}
