using System.ComponentModel.DataAnnotations;
namespace DomainModel.ViewModels.Province;
public class ProvinceAddEditModel
{
    [Display(Name="نام استان")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][StringLength(100)] public string ProvinceName { get; set; } = string.Empty;
    [Display(Name="فعال")] public bool IsActive { get; set; } = true;
    [Display(Name="ترتیب نمایش")] public int DisplayOrder { get; set; }
    [Display(Name="اسلاگ")] public string? Slug { get; set; }
    [Display(Name="عنوان سئو")] public string? SeoTitle { get; set; }
    [Display(Name="توضیحات سئو")] public string? SeoDescription { get; set; }
    [Display(Name="کلمات کلیدی سئو")] public string? SeoKeywords { get; set; }
    [Display(Name="نشانی کانونیکال")] public string? CanonicalUrl { get; set; }
    [Display(Name="تصویر شبکه اجتماعی")] public string? OpenGraphImageUrl { get; set; }
    [Display(Name="قابل ایندکس")] public bool? IsIndexable { get; set; }
    [Display(Name="دنبال کردن لینک‌ها")] public bool? IsFollow { get; set; }
}
