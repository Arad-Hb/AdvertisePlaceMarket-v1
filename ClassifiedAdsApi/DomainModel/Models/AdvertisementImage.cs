namespace DomainModel.Models;

public class AdvertisementImage
{
    public long AdvertisementImageID { get; set; }
    public long AdvertisementID { get; set; }
    public string ImageName { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? AltText { get; set; }
    public bool IsMainImage { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public Advertisement Advertisement { get; set; } = null!;
}
