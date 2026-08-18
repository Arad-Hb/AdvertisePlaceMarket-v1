using System.ComponentModel.DataAnnotations;
namespace DomainModel.ViewModels.HeroBanner;
public class HeroBannerAddEditModel
{
    [Display(Name="عنوان")][StringLength(200)] public string? Title { get; set; }
    [Display(Name="زیرعنوان")][StringLength(500)] public string? Subtitle { get; set; }
    public string? ImagePath { get; set; }
    [Display(Name="لینک")][StringLength(500)] public string? LinkUrl { get; set; }
    [Display(Name="متن دکمه")][StringLength(100)] public string? ButtonText { get; set; }
    [Display(Name="ترتیب نمایش")] public int SortOrder { get; set; }
    [Display(Name="فعال")] public bool IsActive { get; set; }=true;
}
