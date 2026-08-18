namespace DomainModel.Models;

public class HeroBanner
{
    public long HeroBannerID { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? ImagePath { get; set; }
    public string? LinkUrl { get; set; }
    public string? ButtonText { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; }
}
