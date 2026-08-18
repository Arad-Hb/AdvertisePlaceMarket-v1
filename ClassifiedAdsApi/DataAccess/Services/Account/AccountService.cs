using DomainModel.Models; using DomainModel.ViewModels.Account; using Framework.Common; using Microsoft.AspNetCore.Identity;
namespace DataAccess.Services.Account;
public class AccountService(UserManager<ApplicationUser> userManager,IJwtTokenGenerator jwtTokenGenerator):IAccountService
{
 public async Task<OperationResult> RegisterAsync(RegisterModel model)
 {
   var result=new OperationResult("ثبت‌نام");
   if(await userManager.FindByNameAsync(model.MobileNumber) is not null) return result.ToFailed("این شماره موبایل قبلاً ثبت شده است.");
   var user=new ApplicationUser{Id=Guid.NewGuid().ToString(),FirstName=model.FirstName.Trim(),LastName=model.LastName.Trim(),UserName=model.MobileNumber,PhoneNumber=model.MobileNumber,Email=string.IsNullOrWhiteSpace(model.Email)?null:model.Email.Trim(),IsActive=true,CreateDate=DateTime.Now};
   var create=await userManager.CreateAsync(user,model.Password); if(!create.Succeeded) return result.ToFailed(string.Join(" | ",create.Errors.Select(x=>x.Description)));
   var role=await userManager.AddToRoleAsync(user,"Customer"); if(!role.Succeeded){await userManager.DeleteAsync(user); return result.ToFailed("ثبت نقش کاربر انجام نشد.");}
   return result.ToSuccess("ثبت‌نام با موفقیت انجام شد.");
 }
 public async Task<LoginResultModel?> LoginAsync(LoginModel model)
 {
   var user=await userManager.FindByNameAsync(model.MobileNumber); if(user is null||!user.IsActive) return null;
   if(!await userManager.CheckPasswordAsync(user,model.Password)) return null;
   var roles=await userManager.GetRolesAsync(user); return jwtTokenGenerator.Generate(user,roles,model.RememberMe);
 }
 public Task<OperationResult> LogoutAsync(string? userId)=>Task.FromResult(new OperationResult("خروج").ToSuccess("خروج با موفقیت انجام شد."));
 public async Task<AuthenticatedUserModel?> GetAuthenticatedUserAsync(string userId){var user=await userManager.FindByIdAsync(userId); if(user is null||!user.IsActive)return null;var roles=await userManager.GetRolesAsync(user);return new(){UserID=user.Id,FirstName=user.FirstName,LastName=user.LastName,MobileNumber=user.PhoneNumber??user.UserName??string.Empty,AvatarPath=user.AvatarPath,Roles=roles.ToList()};}
 public async Task<OperationResult> ChangePasswordAsync(string userId,ChangePasswordModel model){var r=new OperationResult("تغییر رمز عبور");var user=await userManager.FindByIdAsync(userId);if(user is null)return r.ToFailed("کاربر پیدا نشد.");var changed=await userManager.ChangePasswordAsync(user,model.CurrentPassword,model.NewPassword);return changed.Succeeded?r.ToSuccess("رمز عبور با موفقیت تغییر کرد."):r.ToFailed(string.Join(" | ",changed.Errors.Select(x=>x.Description)));}
 public async Task<OperationResult> UpdateAvatarPathAsync(string userId,string avatarPath){var r=new OperationResult("تصویر کاربر");var user=await userManager.FindByIdAsync(userId);if(user is null)return r.ToFailed("کاربر پیدا نشد.");user.AvatarPath=avatarPath;var update=await userManager.UpdateAsync(user);return update.Succeeded?r.ToSuccess("تصویر کاربر ذخیره شد."):r.ToFailed("ذخیره تصویر کاربر انجام نشد.");}
 public async Task<string?> GetAvatarPathAsync(string userId)=>(await userManager.FindByIdAsync(userId))?.AvatarPath;
}
