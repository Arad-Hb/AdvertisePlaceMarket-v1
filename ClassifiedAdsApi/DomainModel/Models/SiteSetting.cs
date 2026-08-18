namespace DomainModel.Models;

public class SiteSetting
{
    public int SiteSettingID { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string? SiteDescription { get; set; }
    public string? LogoPath { get; set; }
    public string? FaviconPath { get; set; }
    public string? FooterText { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TelegramUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? DefaultSeoTitle { get; set; }
    public string? DefaultSeoDescription { get; set; }
    public string? DefaultSeoKeywords { get; set; }
    public bool IsSiteActive { get; set; } = true;
}
