using DomainModel.Models;
namespace DataAccess.Repositories.Admin;
public interface IAdminRepository
{
 IQueryable<ApplicationUser> CustomerQuery(bool tracking=false);
 Task<ApplicationUser?> GetCustomerAsync(string userId);
 Task<int> CountCustomersAsync();
 Task<int> SaveChangesAsync();
}
