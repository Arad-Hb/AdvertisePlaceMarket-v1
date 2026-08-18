using DomainModel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DomainModel.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Advertisement> Advertisements => Set<Advertisement>();
    public DbSet<AdvertisementCategory> AdvertisementCategories => Set<AdvertisementCategory>();
    public DbSet<AdvertisementStatus> AdvertisementStatuses => Set<AdvertisementStatus>();
    public DbSet<AdvertisementImage> AdvertisementImages => Set<AdvertisementImage>();
    public DbSet<FavoriteAdvertisement> FavoriteAdvertisements => Set<FavoriteAdvertisement>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();
    public DbSet<UserMembership> UserMemberships => Set<UserMembership>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<HeroBanner> HeroBanners => Set<HeroBanner>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(80).IsRequired();
            entity.Property(x => x.AvatarPath).HasMaxLength(500);
            entity.HasIndex(x => x.PhoneNumber);
        });
    }
}
