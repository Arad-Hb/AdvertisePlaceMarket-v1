namespace DomainModel.ViewModels.Advertisement;
public class CustomerAdvertisementDetailsModel : AdvertisementDetailsModel
{
    public int AdvertisementStatusID { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public string StatusTitle { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
}
