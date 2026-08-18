using System.ComponentModel.DataAnnotations;
namespace DomainModel.ViewModels.Advertisement;
public class AdvertisementRejectModel
{
    [Display(Name="دلیل رد آگهی")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][StringLength(1000)] public string RejectionReason { get; set; } = string.Empty;
}
