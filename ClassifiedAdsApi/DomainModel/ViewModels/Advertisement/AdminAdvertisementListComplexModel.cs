using Framework.Common;
namespace DomainModel.ViewModels.Advertisement;
public class AdminAdvertisementListComplexModel
{
    public List<AdminAdvertisementListItem> Items { get; set; } = new();
    public PageModel PageModel { get; set; } = new();
}
