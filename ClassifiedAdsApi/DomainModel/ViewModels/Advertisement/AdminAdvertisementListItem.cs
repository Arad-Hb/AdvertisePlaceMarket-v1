namespace DomainModel.ViewModels.Advertisement;
public class AdminAdvertisementListItem : CustomerAdvertisementListItem
{
    public string UserID { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerMobileNumber { get; set; } = string.Empty;
}
