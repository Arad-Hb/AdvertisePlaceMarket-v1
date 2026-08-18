using Framework.Common;
namespace DomainModel.ViewModels.AdvertisementCategory;
public class AdvertisementCategorySearchModel : PageModel
{
    public string? Keyword { get; set; }
    public int? Depth { get; set; }
    public bool? IsActive { get; set; }
}
