using Framework.Common;
namespace DomainModel.ViewModels.Advertisement;
public class AdminAdvertisementSearchModel : PageModel
{
    public string? Keyword { get; set; }
    public int? AdvertisementStatusID { get; set; }
    public int? AdvertisementCategoryID { get; set; }
    public int? ProvinceID { get; set; }
    public int? CityID { get; set; }
    public string? CustomerUserID { get; set; }
    public bool? IsImmediate { get; set; }
    public bool? IsFeatured { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Sort { get; set; } = "newest";
}
