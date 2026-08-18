namespace DomainModel.ViewModels.Admin;
public class AdminDashboardModel
{
    public int TotalCustomers { get; set; }
    public int TotalAdvertisements { get; set; }
    public int PublishedAdvertisements { get; set; }
    public int PendingAdvertisements { get; set; }
    public int RejectedAdvertisements { get; set; }
    public int ImmediateAdvertisements { get; set; }
    public int FeaturedAdvertisements { get; set; }
    public int ActiveMemberships { get; set; }
    public int TotalPayments { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TodayAdvertisements { get; set; }
    public int TodayPayments { get; set; }
}
