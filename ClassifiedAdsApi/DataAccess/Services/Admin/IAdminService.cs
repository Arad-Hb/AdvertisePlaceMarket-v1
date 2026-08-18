using DomainModel.ViewModels.Admin;
using Framework.Common;

namespace DataAccess.Services.Admin;

public interface IAdminService
{
    Task<AdminDashboardModel> GetDashboardAsync();
    Task<CustomerListComplexModel> SearchCustomersAsync(CustomerSearchModel model);
    Task<CustomerListItem?> GetCustomerAsync(string userId);
    Task<OperationResult> AddCustomerAsync(CustomerAddEditModel model);
    Task<OperationResult> UpdateCustomerAsync(string userId, CustomerAddEditModel model);
    Task<OperationResult> DeleteCustomerAsync(string userId);
    Task<OperationResult> SetCustomerActiveAsync(string userId, bool active);
}
