using DomainModel.Context; using DomainModel.Models; using Microsoft.EntityFrameworkCore;
namespace DataAccess.Repositories.Admin;
public class AdminRepository(ApplicationDbContext context):IAdminRepository
{
 public IQueryable<ApplicationUser> CustomerQuery(bool tracking=false)
 {
   var customerRoleId=context.Roles.Where(r=>r.Name=="Customer").Select(r=>r.Id).FirstOrDefault();
   var userIds=context.UserRoles.Where(ur=>ur.RoleId==customerRoleId).Select(ur=>ur.UserId);
   var q=context.Users.Where(u=>userIds.Contains(u.Id));
   return tracking?q:q.AsNoTracking();
 }
 public Task<ApplicationUser?> GetCustomerAsync(string userId)=>CustomerQuery(true).FirstOrDefaultAsync(x=>x.Id==userId);
 public Task<int> CountCustomersAsync()=>CustomerQuery().CountAsync();
 public Task<int> SaveChangesAsync()=>context.SaveChangesAsync();
}
