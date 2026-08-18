using Microsoft.AspNetCore.Identity;

namespace DomainModel.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarPath { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreateDate { get; set; } = DateTime.Now;

    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
    public ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<FavoriteAdvertisement> FavoriteAdvertisements { get; set; } = new List<FavoriteAdvertisement>();
}
