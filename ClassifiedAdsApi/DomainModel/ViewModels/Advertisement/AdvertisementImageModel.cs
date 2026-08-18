namespace DomainModel.ViewModels.Advertisement;
public class AdvertisementImageModel
{
    public long AdvertisementImageID { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? AltText { get; set; }
    public bool IsMainImage { get; set; }
    public int DisplayOrder { get; set; }
}
