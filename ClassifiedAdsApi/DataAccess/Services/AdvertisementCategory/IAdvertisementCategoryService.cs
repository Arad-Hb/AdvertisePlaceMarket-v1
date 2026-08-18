using DomainModel.ViewModels.AdvertisementCategory; using Framework.Common;
namespace DataAccess.Services.AdvertisementCategory;
public interface IAdvertisementCategoryService
{
 Task<List<AdvertisementCategoryMenuItem>> GetMenuAsync();
 Task<AdvertisementCategoryDetailsModel?> GetDetailsAsync(int id);
 Task<AdvertisementCategoryDetailsModel?> GetBySlugAsync(string slug);
 Task<AdvertisementCategoryListComplexModel> SearchAsync(AdvertisementCategorySearchModel model);
 Task<OperationResult> AddAsync(AdvertisementCategoryAddEditModel model);
 Task<OperationResult> UpdateAsync(int id,AdvertisementCategoryAddEditModel model);
 Task<OperationResult> DeleteAsync(int id);
}
