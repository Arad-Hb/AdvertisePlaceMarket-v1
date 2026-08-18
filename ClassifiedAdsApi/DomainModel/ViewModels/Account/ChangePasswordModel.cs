using System.ComponentModel.DataAnnotations;
namespace DomainModel.ViewModels.Account;
public class ChangePasswordModel
{
    [Display(Name="رمز عبور فعلی")][Required(ErrorMessage="وارد کردن {0} الزامی است.")] public string CurrentPassword { get; set; } = string.Empty;
    [Display(Name="رمز عبور جدید")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][MinLength(6,ErrorMessage="{0} باید حداقل {1} کاراکتر باشد.")] public string NewPassword { get; set; } = string.Empty;
    [Display(Name="تکرار رمز عبور جدید")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][Compare(nameof(NewPassword),ErrorMessage="رمز عبور جدید و تکرار آن یکسان نیستند.")] public string ConfirmNewPassword { get; set; } = string.Empty;
}
