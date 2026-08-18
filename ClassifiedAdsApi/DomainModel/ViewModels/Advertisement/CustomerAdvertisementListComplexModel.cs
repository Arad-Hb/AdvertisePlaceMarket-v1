using Framework.Common;
namespace DomainModel.ViewModels.Advertisement;
public class CustomerAdvertisementListComplexModel
{
    public List<CustomerAdvertisementListItem> Items { get; set; } = new();
    public PageModel PageModel { get; set; } = new();
}
