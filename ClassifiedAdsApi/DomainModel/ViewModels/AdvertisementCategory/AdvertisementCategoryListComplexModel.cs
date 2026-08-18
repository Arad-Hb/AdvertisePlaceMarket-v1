using Framework.Common;
namespace DomainModel.ViewModels.AdvertisementCategory;
public class AdvertisementCategoryListComplexModel
{
    public List<AdvertisementCategoryListItem> Items { get; set; } = new();
    public PageModel PageModel { get; set; } = new();
}
