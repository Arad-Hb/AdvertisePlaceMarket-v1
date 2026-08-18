using System.ComponentModel.DataAnnotations;
namespace DomainModel.ViewModels.Account;
public class LoginModel
{
    [Display(Name="شماره موبایل")][Required(ErrorMessage="وارد کردن {0} الزامی است.")][RegularExpression(@"^09\d{9}$",ErrorMessage="{0} معتبر نیست.")] public string MobileNumber { get; set; } = string.Empty;
    [Display(Name="رمز عبور")][Required(ErrorMessage="وارد کردن {0} الزامی است.")] public string Password { get; set; } = string.Empty;
    [Display(Name="مرا به خاطر بسپار")] public bool RememberMe { get; set; }
}
