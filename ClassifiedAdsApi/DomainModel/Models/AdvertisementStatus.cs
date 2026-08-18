namespace DomainModel.Models;

public class AdvertisementStatus
{
    public int AdvertisementStatusID { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
