namespace DomainModel.Models;

public class FavoriteAdvertisement
{
    public long FavoriteAdvertisementID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public long AdvertisementID { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public ApplicationUser User { get; set; } = null!;
    public Advertisement Advertisement { get; set; } = null!;
}
