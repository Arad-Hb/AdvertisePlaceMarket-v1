using DataAccess.Repositories.Admin;
using DataAccess.Repositories.Advertisement;
using DataAccess.Repositories.Membership;
using DataAccess.Repositories.Payment;
using DataAccess.Services.Common;
using DomainModel.Models;
using DomainModel.ViewModels.Admin;
using Framework.Common;
using Framework.Common.Constants;
using Framework.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services.Admin;

public class AdminService(
    IAdminRepository adminRepository,
    IAdvertisementRepository advertisementRepository,
    IMembershipRepository membershipRepository,
    IPaymentRepository paymentRepository,
    IPaginationService pagination,
    UserManager<ApplicationUser> userManager) : IAdminService
{
    public async Task<AdminDashboardModel> GetDashboardAsync()
    {
        var today = DateTime.Today;
        var ads = advertisementRepository.Query();
        var payments = paymentRepository.Query();
        var memberships = membershipRepository.UserMembershipQuery();
        return new AdminDashboardModel
        {
            TotalCustomers = await adminRepository.CountCustomersAsync(),
            TotalAdvertisements = await ads.CountAsync(),
            PublishedAdvertisements = await ads.CountAsync(x => x.AdvertisementStatus.Code == AdvertisementStatusCodes.Published),
            PendingAdvertisements = await ads.CountAsync(x => x.AdvertisementStatus.Code == AdvertisementStatusCodes.Pending),
            RejectedAdvertisements = await ads.CountAsync(x => x.AdvertisementStatus.Code == AdvertisementStatusCodes.Rejected),
            ImmediateAdvertisements = await ads.CountAsync(x => x.IsImmediate),
            FeaturedAdvertisements = await ads.CountAsync(x => x.IsFeatured),
            ActiveMemberships = await memberships.CountAsync(x => x.IsActive && x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now),
            TotalPayments = await payments.CountAsync(),
            TotalRevenue = await payments.Where(x => x.IsPaid).SumAsync(x => (decimal?)x.Amount) ?? 0,
            TodayAdvertisements = await ads.CountAsync(x => x.CreateDate >= today),
            TodayPayments = await payments.CountAsync(x => x.CreateDate >= today)
        };
    }

    public async Task<CustomerListComplexModel> SearchCustomersAsync(CustomerSearchModel m)
    {
        var q = adminRepository.CustomerQuery();
        if (!string.IsNullOrWhiteSpace(m.Keyword))
        {
            var k = m.Keyword.Trim();
            q = q.Where(x => (x.FirstName + " " + x.LastName).Contains(k) || (x.PhoneNumber ?? "").Contains(k) || (x.Email ?? "").Contains(k));
        }
        if (m.IsActive.HasValue)
            q = q.Where(x => x.IsActive == m.IsActive);

        var p = q.OrderByDescending(x => x.CreateDate).Select(x => new CustomerListItem
        {
            UserID = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            MobileNumber = x.PhoneNumber ?? string.Empty,
            Email = x.Email,
            AvatarPath = x.AvatarPath,
            IsActive = x.IsActive,
            CreateDate = x.CreateDate
        });
        var items = await pagination.PaginateAsync(p, m);
        foreach (var x in items)
            x.CreateDatePersian = x.CreateDate.ToPersianDateTime();
        return new() { Items = items, PageModel = m };
    }

    public async Task<CustomerListItem?> GetCustomerAsync(string userId)
    {
        var user = await adminRepository.GetCustomerAsync(userId);
        if (user is null) return null;
        var item = MapCustomer(user);
        item.CreateDatePersian = item.CreateDate.ToPersianDateTime();
        return item;
    }

    public async Task<OperationResult> AddCustomerAsync(CustomerAddEditModel model)
    {
        var result = new OperationResult("ثبت مشتری");
        var mobile = model.MobileNumber.Trim();
        if (string.IsNullOrWhiteSpace(model.Password))
            return result.ToFailed("رمز عبور الزامی است.");
        if (await userManager.FindByNameAsync(mobile) is not null)
            return result.ToFailed("این شماره موبایل قبلاً ثبت شده است.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            UserName = mobile,
            PhoneNumber = mobile,
            Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim(),
            IsActive = model.IsActive,
            CreateDate = DateTime.Now
        };
        var create = await userManager.CreateAsync(user, model.Password);
        if (!create.Succeeded)
            return result.ToFailed(string.Join(" | ", create.Errors.Select(x => x.Description)));

        var role = await userManager.AddToRoleAsync(user, "Customer");
        if (!role.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return result.ToFailed("ثبت نقش کاربر انجام نشد.");
        }
        return result.ToSuccess("مشتری با موفقیت ثبت شد.");
    }

    public async Task<OperationResult> UpdateCustomerAsync(string userId, CustomerAddEditModel model)
    {
        var result = new OperationResult("ویرایش مشتری");
        var user = await adminRepository.GetCustomerAsync(userId);
        if (user is null)
            return result.ToFailed("مشتری پیدا نشد.");

        var mobile = model.MobileNumber.Trim();
        var existing = await userManager.FindByNameAsync(mobile);
        if (existing is not null && existing.Id != user.Id)
            return result.ToFailed("این شماره موبایل قبلاً ثبت شده است.");

        user.FirstName = model.FirstName.Trim();
        user.LastName = model.LastName.Trim();
        user.UserName = mobile;
        user.PhoneNumber = mobile;
        user.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
        user.IsActive = model.IsActive;

        var update = await userManager.UpdateAsync(user);
        if (!update.Succeeded)
            return result.ToFailed(string.Join(" | ", update.Errors.Select(x => x.Description)));

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var reset = await userManager.ResetPasswordAsync(user, token, model.Password);
            if (!reset.Succeeded)
                return result.ToFailed(string.Join(" | ", reset.Errors.Select(x => x.Description)));
        }
        return result.ToSuccess("اطلاعات مشتری ذخیره شد.");
    }

    public async Task<OperationResult> DeleteCustomerAsync(string userId)
    {
        var result = new OperationResult("حذف مشتری");
        var user = await adminRepository.GetCustomerAsync(userId);
        if (user is null)
            return result.ToFailed("مشتری پیدا نشد.");

        await adminRepository.DeleteCustomerRelatedDataAsync(userId);
        var deleted = await userManager.DeleteAsync(user);
        return deleted.Succeeded
            ? result.ToSuccess("مشتری حذف شد.")
            : result.ToFailed(string.Join(" | ", deleted.Errors.Select(x => x.Description)));
    }

    public async Task<OperationResult> SetCustomerActiveAsync(string userId, bool active)
    {
        var r = new OperationResult(active ? "فعال‌سازی مشتری" : "غیرفعال‌سازی مشتری");
        var user = await adminRepository.GetCustomerAsync(userId);
        if (user is null)
            return r.ToFailed("مشتری پیدا نشد.");
        user.IsActive = active;
        await adminRepository.SaveChangesAsync();
        return r.ToSuccess(active ? "مشتری فعال شد." : "مشتری غیرفعال شد.");
    }

    private static CustomerListItem MapCustomer(ApplicationUser x) => new()
    {
        UserID = x.Id,
        FirstName = x.FirstName,
        LastName = x.LastName,
        MobileNumber = x.PhoneNumber ?? string.Empty,
        Email = x.Email,
        AvatarPath = x.AvatarPath,
        IsActive = x.IsActive,
        CreateDate = x.CreateDate
    };
}
