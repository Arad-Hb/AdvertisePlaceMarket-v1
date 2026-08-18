using System.ComponentModel.DataAnnotations;
namespace DomainModel.ViewModels.AdvertisementCategory;
public class AdvertisementCategoryAddEditModel
{
    [Display(Name="نام دسته‌بندی")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][StringLength(120)] public string CategoryName { get; set; } = string.Empty;
    [Display(Name="دسته والد")] public int? ParentID { get; set; }
    [Display(Name="ترتیب نمایش")] public int? SortOrder { get; set; }
    [Display(Name="اسلاگ")][StringLength(200)] public string? Slug { get; set; }
    [Display(Name="عنوان سئو")][StringLength(200)] public string? SeoTitle { get; set; }
    [Display(Name="توضیحات سئو")][StringLength(500)] public string? SeoDescription { get; set; }
    [Display(Name="کلمات کلیدی سئو")][StringLength(500)] public string? SeoKeywords { get; set; }
    [Display(Name="نشانی کانونیکال")][StringLength(500)] public string? CanonicalUrl { get; set; }
    [Display(Name="تصویر شبکه اجتماعی")][StringLength(500)] public string? OpenGraphImageUrl { get; set; }
    [Display(Name="قابل ایندکس")] public bool? IsIndexable { get; set; }
    [Display(Name="دنبال کردن لینک‌ها")] public bool? IsFollow { get; set; }
    [Display(Name="فعال")] public bool IsActive { get; set; } = true;
    [Display(Name="آیکن")][StringLength(200)] public string? Icon { get; set; }
}
