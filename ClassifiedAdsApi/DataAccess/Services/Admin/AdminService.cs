using DataAccess.Repositories.Admin; using DataAccess.Repositories.Advertisement; using DataAccess.Repositories.Membership; using DataAccess.Repositories.Payment; using DataAccess.Services.Common; using DomainModel.ViewModels.Admin; using Framework.Common; using Framework.Common.Constants; using Framework.Common.Extensions; using Microsoft.EntityFrameworkCore;
namespace DataAccess.Services.Admin;
public class AdminService(IAdminRepository adminRepository,IAdvertisementRepository advertisementRepository,IMembershipRepository membershipRepository,IPaymentRepository paymentRepository,IPaginationService pagination):IAdminService
{
 public async Task<AdminDashboardModel> GetDashboardAsync()
 {
  var today=DateTime.Today;var ads=advertisementRepository.Query();var payments=paymentRepository.Query();var memberships=membershipRepository.UserMembershipQuery();
  return new AdminDashboardModel{
   TotalCustomers=await adminRepository.CountCustomersAsync(),TotalAdvertisements=await ads.CountAsync(),
   PublishedAdvertisements=await ads.CountAsync(x=>x.AdvertisementStatus.Code==AdvertisementStatusCodes.Published),
   PendingAdvertisements=await ads.CountAsync(x=>x.AdvertisementStatus.Code==AdvertisementStatusCodes.Pending),
   RejectedAdvertisements=await ads.CountAsync(x=>x.AdvertisementStatus.Code==AdvertisementStatusCodes.Rejected),
   ImmediateAdvertisements=await ads.CountAsync(x=>x.IsImmediate),FeaturedAdvertisements=await ads.CountAsync(x=>x.IsFeatured),
   ActiveMemberships=await memberships.CountAsync(x=>x.IsActive&&x.StartDate<=DateTime.Now&&x.EndDate>=DateTime.Now),
   TotalPayments=await payments.CountAsync(),TotalRevenue=await payments.Where(x=>x.IsPaid).SumAsync(x=>(decimal?)x.Amount)??0,
   TodayAdvertisements=await ads.CountAsync(x=>x.CreateDate>=today),TodayPayments=await payments.CountAsync(x=>x.CreateDate>=today)
  };
 }
 public async Task<CustomerListComplexModel> SearchCustomersAsync(CustomerSearchModel m)
 {
   var q=adminRepository.CustomerQuery();if(!string.IsNullOrWhiteSpace(m.Keyword)){var k=m.Keyword.Trim();q=q.Where(x=>(x.FirstName+" "+x.LastName).Contains(k)||(x.PhoneNumber??"").Contains(k));}if(m.IsActive.HasValue)q=q.Where(x=>x.IsActive==m.IsActive);
   var p=q.OrderByDescending(x=>x.CreateDate).Select(x=>new CustomerListItem{UserID=x.Id,FirstName=x.FirstName,LastName=x.LastName,MobileNumber=x.PhoneNumber??string.Empty,AvatarPath=x.AvatarPath,IsActive=x.IsActive,CreateDate=x.CreateDate,CreateDatePersian=""});var items=await pagination.PaginateAsync(p,m);foreach(var x in items)x.CreateDatePersian=x.CreateDate.ToPersianDateTime();return new(){Items=items,PageModel=m};
 }
 public async Task<OperationResult> SetCustomerActiveAsync(string userId,bool active){var r=new OperationResult(active?"فعال‌سازی مشتری":"غیرفعال‌سازی مشتری");var user=await adminRepository.GetCustomerAsync(userId);if(user is null)return r.ToFailed("مشتری پیدا نشد.");user.IsActive=active;await adminRepository.SaveChangesAsync();return r.ToSuccess(active?"مشتری فعال شد.":"مشتری غیرفعال شد.");}
}