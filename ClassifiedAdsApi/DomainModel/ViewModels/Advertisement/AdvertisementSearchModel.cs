using Framework.Common;
namespace DomainModel.ViewModels.Advertisement;
public class AdvertisementSearchModel : PageModel
{
    public string? Keyword { get; set; }
    public int? AdvertisementCategoryID { get; set; }
    public int? ProvinceID { get; set; }
    public int? CityID { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsImmediate { get; set; }
    public string? Sort { get; set; } = "newest";
}
