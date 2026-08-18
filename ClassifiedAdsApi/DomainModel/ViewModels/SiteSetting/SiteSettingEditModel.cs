using System.ComponentModel.DataAnnotations;
namespace DomainModel.ViewModels.SiteSetting;
public class SiteSettingEditModel
{
    [Display(Name="نام سایت")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][StringLength(150)] public string SiteName { get; set; }=string.Empty;
    [Display(Name="توضیحات سایت")][StringLength(500)] public string? SiteDescription { get; set; }
    public string? LogoPath { get; set; }
    public string? FaviconPath { get; set; }
    [Display(Name="متن فوتر")][StringLength(1000)] public string? FooterText { get; set; }
    [Display(Name="اینستاگرام")] public string? InstagramUrl { get; set; }
    [Display(Name="تلگرام")] public string? TelegramUrl { get; set; }
    [Display(Name="لینکدین")] public string? LinkedInUrl { get; set; }
    [Display(Name="عنوان پیش‌فرض سئو")] public string? DefaultSeoTitle { get; set; }
    [Display(Name="توضیحات پیش‌فرض سئو")] public string? DefaultSeoDescription { get; set; }
    [Display(Name="کلمات کلیدی پیش‌فرض سئو")] public string? DefaultSeoKeywords { get; set; }
    [Display(Name="سایت فعال باشد")] public bool IsSiteActive { get; set; }=true;
}
