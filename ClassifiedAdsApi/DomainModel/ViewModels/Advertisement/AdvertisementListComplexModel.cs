using DomainModel.ViewModels.Common;
using Framework.Common;
namespace DomainModel.ViewModels.Advertisement;
public class AdvertisementListComplexModel
{
    public List<AdvertisementListItem> Items { get; set; } = new();
    public PageModel PageModel { get; set; } = new();
    public List<BreadcrumbItemModel>? Breadcrumb { get; set; }
}
