using System.ComponentModel.DataAnnotations;
namespace DomainModel.ViewModels.Membership;
public class MembershipPlanAddEditModel
{
    [Display(Name="عنوان عضویت")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][StringLength(100)] public string Title { get; set; }=string.Empty;
    [Display(Name="توضیحات")][StringLength(1000)] public string? Description { get; set; }
    [Display(Name="مدت اعتبار (روز)")][Range(1,3650,ErrorMessage="{0} باید بیشتر از صفر باشد.")] public int DurationDays { get; set; }
    [Display(Name="تعداد آگهی مجاز")][Range(1,10000,ErrorMessage="{0} باید بیشتر از صفر باشد.")] public int AdvertisementLimit { get; set; }
    [Display(Name="قیمت")][Range(0,double.MaxValue,ErrorMessage="{0} نمی‌تواند منفی باشد.")] public decimal Price { get; set; }
    [Display(Name="فعال")] public bool IsActive { get; set; }=true;
    [Display(Name="ترتیب نمایش")] public int SortOrder { get; set; }
}
