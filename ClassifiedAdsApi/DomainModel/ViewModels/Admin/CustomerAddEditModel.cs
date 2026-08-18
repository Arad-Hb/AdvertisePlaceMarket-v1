using System.ComponentModel.DataAnnotations;

namespace DomainModel.ViewModels.Admin;

public class CustomerAddEditModel
{
    [Display(Name = "نام")]
    [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "نام خانوادگی")]
    [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "شماره موبایل")]
    [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
    [RegularExpression(@"^09\d{9}$", ErrorMessage = "{0} معتبر نیست.")]
    public string MobileNumber { get; set; } = string.Empty;

    [Display(Name = "ایمیل")]
    [EmailAddress(ErrorMessage = "{0} معتبر نیست.")]
    public string? Email { get; set; }

    [Display(Name = "رمز عبور")]
    [MinLength(6, ErrorMessage = "{0} باید حداقل {1} کاراکتر باشد.")]
    public string? Password { get; set; }

    [Display(Name = "تکرار رمز عبور")]
    [Compare(nameof(Password), ErrorMessage = "رمز عبور و تکرار آن یکسان نیستند.")]
    public string? ConfirmPassword { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; } = true;
}
