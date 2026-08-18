using System.ComponentModel.DataAnnotations;
namespace DomainModel.ViewModels.Advertisement;
public class AdvertisementEditModel
{
    [Display(Name="عنوان آگهی")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][StringLength(150,ErrorMessage="{0} نمی‌تواند بیشتر از {1} کاراکتر باشد.")] public string Title { get; set; } = string.Empty;
    [Display(Name="توضیحات آگهی")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][StringLength(4000)] public string Description { get; set; } = string.Empty;
    [Display(Name="قیمت")][Range(0,double.MaxValue,ErrorMessage="{0} نمی‌تواند منفی باشد.")] public decimal? Price { get; set; }
    [Display(Name="شماره تماس")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][RegularExpression(@"^09\d{9}$",ErrorMessage="{0} معتبر نیست.")] public string PhoneNumber { get; set; } = string.Empty;
    [Display(Name="دسته‌بندی")][Range(1,int.MaxValue,ErrorMessage="انتخاب {0} الزامی است.")] public int AdvertisementCategoryID { get; set; }
    [Display(Name="استان")][Range(1,int.MaxValue,ErrorMessage="انتخاب {0} الزامی است.")] public int ProvinceID { get; set; }
    [Display(Name="شهر")][Range(1,int.MaxValue,ErrorMessage="انتخاب {0} الزامی است.")] public int CityID { get; set; }
    [Display(Name="فوری")] public bool IsImmediate { get; set; }
    [Display(Name="عنوان سئو")][StringLength(200)] public string? SeoTitle { get; set; }
    [Display(Name="توضیحات سئو")][StringLength(500)] public string? SeoDescription { get; set; }
    [Display(Name="کلمات کلیدی سئو")][StringLength(500)] public string? SeoKeywords { get; set; }
    [Display(Name="نشانی کانونیکال")][StringLength(500)] public string? CanonicalUrl { get; set; }
    [Display(Name="تصویر شبکه اجتماعی")][StringLength(500)] public string? OpenGraphImageUrl { get; set; }
    [Display(Name="قابل ایندکس")] public bool? IsIndexable { get; set; }
    [Display(Name="دنبال کردن لینک‌ها")] public bool? IsFollow { get; set; }
}
