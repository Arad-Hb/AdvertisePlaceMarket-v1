using DomainModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DomainModel.Configurations;
public class AdvertisementConfiguration : IEntityTypeConfiguration<Advertisement>
{
    public void Configure(EntityTypeBuilder<Advertisement> b)
    {
        b.HasKey(x=>x.AdvertisementID);
        b.Property(x=>x.Title).HasMaxLength(150).IsRequired();
        b.Property(x=>x.Description).HasMaxLength(4000).IsRequired();
        b.Property(x=>x.PhoneNumber).HasMaxLength(20).IsRequired();
        b.Property(x=>x.Price).HasPrecision(18,0);
        b.Property(x=>x.Slug).HasMaxLength(250);
        b.Property(x=>x.SeoTitle).HasMaxLength(200); b.Property(x=>x.SeoDescription).HasMaxLength(500); b.Property(x=>x.SeoKeywords).HasMaxLength(500); b.Property(x=>x.CanonicalUrl).HasMaxLength(500); b.Property(x=>x.OpenGraphImageUrl).HasMaxLength(500); b.Property(x=>x.RejectionReason).HasMaxLength(1000);
        b.HasIndex(x=>x.Slug).IsUnique().HasFilter("[Slug] IS NOT NULL");
        b.HasIndex(x=>x.AdvertisementStatusID); b.HasIndex(x=>x.AdvertisementCategoryID); b.HasIndex(x=>x.ProvinceID); b.HasIndex(x=>x.CityID); b.HasIndex(x=>x.UserID); b.HasIndex(x=>x.UserMembershipID); b.HasIndex(x=>x.PublishDate); b.HasIndex(x=>x.Price); b.HasIndex(x=>x.IsImmediate); b.HasIndex(x=>x.IsFeatured);
        b.HasIndex(x=>new{x.AdvertisementStatusID,x.PublishDate});
        b.HasIndex(x=>new{x.AdvertisementCategoryID,x.AdvertisementStatusID,x.PublishDate});
        b.HasIndex(x=>new{x.ProvinceID,x.CityID,x.AdvertisementStatusID});
        b.HasIndex(x=>new{x.UserID,x.CreateDate});
        b.HasOne(x=>x.AdvertisementCategory).WithMany(x=>x.Advertisements).HasForeignKey(x=>x.AdvertisementCategoryID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x=>x.Province).WithMany(x=>x.Advertisements).HasForeignKey(x=>x.ProvinceID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x=>x.City).WithMany(x=>x.Advertisements).HasForeignKey(x=>x.CityID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x=>x.User).WithMany(x=>x.Advertisements).HasForeignKey(x=>x.UserID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x=>x.ReviewedByUser).WithMany().HasForeignKey(x=>x.ReviewedByUserID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x=>x.UserMembership).WithMany(x=>x.Advertisements).HasForeignKey(x=>x.UserMembershipID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x=>x.AdvertisementStatus).WithMany(x=>x.Advertisements).HasForeignKey(x=>x.AdvertisementStatusID).OnDelete(DeleteBehavior.Restrict);
    }
}