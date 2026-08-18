using DataAccess.Repositories.Advertisement;
using DataAccess.Services.Common;
using DomainModel.Models;
using DomainModel.ViewModels.Advertisement;
using Framework.Common;
using Framework.Common.Constants;
using Framework.Common.Extensions;
using Framework.Common.Seo;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services.Advertisement;

public class AdvertisementService(IAdvertisementRepository repository,IPaginationService pagination,IBreadcrumbService breadcrumb) : IAdvertisementService
{
    private static void SetPersianDates(IEnumerable<AdvertisementListItem> items)
    {
        foreach(var item in items) item.CreateDatePersian=item.CreateDate.ToPersianDateTime();
    }

    private static IQueryable<AdvertisementListItem> ProjectPublic(IQueryable<DomainModel.Models.Advertisement> query)
        => query.Select(x=>new AdvertisementListItem
        {
            AdvertisementID=x.AdvertisementID, Title=x.Title, Price=x.Price, IsImmediate=x.IsImmediate, IsFeatured=x.IsFeatured,
            CategoryName=x.AdvertisementCategory.CategoryName, CategorySlug=x.AdvertisementCategory.Slug,
            ProvinceName=x.Province.ProvinceName, CityName=x.City.CityName,
            ThumbnailPath=x.Images.OrderByDescending(i=>i.IsMainImage).ThenBy(i=>i.DisplayOrder).Select(i=>i.ThumbnailPath).FirstOrDefault(),
            CreateDate=x.CreateDate, CreateDatePersian="", Slug=x.Slug
        });

    private async Task<IQueryable<DomainModel.Models.Advertisement>> ApplyPublicFiltersAsync(IQueryable<DomainModel.Models.Advertisement> query,AdvertisementSearchModel model)
    {
        query=query.Where(x=>x.AdvertisementStatus.Code==AdvertisementStatusCodes.Published && x.AdvertisementCategory.IsActive && x.Province.IsActive && x.City.IsActive);
        if(!string.IsNullOrWhiteSpace(model.Keyword))
        {
            var pattern=$"%{model.Keyword.Trim()}%";
            query=query.Where(x=>EF.Functions.Like(x.Title,pattern)||EF.Functions.Like(x.Description,pattern));
        }
        if(model.AdvertisementCategoryID.HasValue)
        {
            var category=await repository.GetCategoryAsync(model.AdvertisementCategoryID.Value);
            if(category is not null)
            {
                if(category.Depth==1)
                    query=query.Where(x=>x.AdvertisementCategory.ParentID==category.AdvertisementCategoryID);
                else query=query.Where(x=>x.AdvertisementCategoryID==category.AdvertisementCategoryID);
            }
            else query=query.Where(x=>false);
        }
        if(model.ProvinceID.HasValue) query=query.Where(x=>x.ProvinceID==model.ProvinceID.Value);
        if(model.CityID.HasValue) query=query.Where(x=>x.CityID==model.CityID.Value);
        if(model.MinPrice.HasValue) query=query.Where(x=>x.Price>=model.MinPrice.Value);
        if(model.MaxPrice.HasValue) query=query.Where(x=>x.Price<=model.MaxPrice.Value);
        if(model.IsImmediate.HasValue) query=query.Where(x=>x.IsImmediate==model.IsImmediate.Value);
        return query;
    }

    private static IQueryable<DomainModel.Models.Advertisement> Sort(IQueryable<DomainModel.Models.Advertisement> q,string? sort)
        => (sort??"newest").ToLowerInvariant() switch
        {
            "oldest"=>q.OrderBy(x=>x.CreateDate),
            "price-asc"=>q.OrderBy(x=>x.Price),
            "price-desc"=>q.OrderByDescending(x=>x.Price),
            "most-viewed"=>q.OrderByDescending(x=>x.ViewCount).ThenByDescending(x=>x.CreateDate),
            _=>q.OrderByDescending(x=>x.CreateDate)
        };

    public async Task<AdvertisementListComplexModel> SearchPublicAsync(AdvertisementSearchModel model)
    {
        var q=await ApplyPublicFiltersAsync(repository.Query(),model);
        q=Sort(q,model.Sort);
        var items=await pagination.PaginateAsync(ProjectPublic(q),model); SetPersianDates(items);
        var crumbs=model.AdvertisementCategoryID.HasValue?await breadcrumb.BuildCategoryAsync(model.AdvertisementCategoryID.Value):null;
        return new(){Items=items,PageModel=model,Breadcrumb=crumbs};
    }

    public async Task<AdvertisementDetailsModel?> GetPublicDetailsAsync(long id,string? currentUserId=null)
    {
        var model=await repository.Query().Where(x=>x.AdvertisementID==id && x.AdvertisementStatus.Code==AdvertisementStatusCodes.Published && x.AdvertisementCategory.IsActive && x.Province.IsActive && x.City.IsActive)
            .Select(x=>new AdvertisementDetailsModel
            {
                AdvertisementID=x.AdvertisementID,Title=x.Title,Description=x.Description,Price=x.Price,IsImmediate=x.IsImmediate,
                AdvertisementCategoryID=x.AdvertisementCategoryID,CategoryName=x.AdvertisementCategory.CategoryName,CategorySlug=x.AdvertisementCategory.Slug,
                ProvinceID=x.ProvinceID,ProvinceName=x.Province.ProvinceName,CityID=x.CityID,CityName=x.City.CityName,PhoneNumber=x.PhoneNumber,
                CreateDate=x.CreateDate,UpdateDate=x.UpdateDate,PublishDate=x.PublishDate,ViewCount=x.ViewCount,Slug=x.Slug,
                SeoTitle=x.SeoTitle,SeoDescription=x.SeoDescription,SeoKeywords=x.SeoKeywords,CanonicalUrl=x.CanonicalUrl,OpenGraphImageUrl=x.OpenGraphImageUrl,IsIndexable=x.IsIndexable,IsFollow=x.IsFollow,
                Images=x.Images.OrderByDescending(i=>i.IsMainImage).ThenBy(i=>i.DisplayOrder).Select(i=>new AdvertisementImageModel{AdvertisementImageID=i.AdvertisementImageID,ImagePath=i.ImagePath,ThumbnailPath=i.ThumbnailPath,Title=i.Title,AltText=i.AltText,IsMainImage=i.IsMainImage,DisplayOrder=i.DisplayOrder}).ToList()
            }).FirstOrDefaultAsync();
        if(model is null)return null;
        model.CreateDatePersian=model.CreateDate.ToPersianDateTime();model.UpdateDatePersian=model.UpdateDate.ToPersianDateTime();model.PublishDatePersian=model.PublishDate.ToPersianDateTime();model.Breadcrumb=await breadcrumb.BuildAdvertisementAsync(model.AdvertisementCategoryID,model.Title);
        if(!string.IsNullOrWhiteSpace(currentUserId)) model.IsFavorite=await repository.FavoriteQuery().AnyAsync(x=>x.UserID==currentUserId&&x.AdvertisementID==id);
        await repository.Query(true).Where(x=>x.AdvertisementID==id).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.ViewCount,x=>x.ViewCount+1));model.ViewCount++;
        return model;
    }

    public async Task<AdvertisementDetailsModel?> GetPublicDetailsBySlugAsync(string slug,string? currentUserId=null)
    {
        var id=await repository.Query().Where(x=>x.Slug==slug&&x.AdvertisementStatus.Code==AdvertisementStatusCodes.Published).Select(x=>(long?)x.AdvertisementID).FirstOrDefaultAsync();
        return id.HasValue?await GetPublicDetailsAsync(id.Value,currentUserId):null;
    }

    public async Task<List<AdvertisementListItem>> GetFeaturedAsync(int count=8){var q=repository.Query().Where(x=>x.AdvertisementStatus.Code==AdvertisementStatusCodes.Published&&x.IsFeatured&&x.AdvertisementCategory.IsActive&&x.Province.IsActive&&x.City.IsActive).OrderByDescending(x=>x.PublishDate).Take(Math.Clamp(count,1,30));var items=await ProjectPublic(q).ToListAsync();SetPersianDates(items);return items;}
    public async Task<List<AdvertisementListItem>> GetLatestAsync(int count=12){var q=repository.Query().Where(x=>x.AdvertisementStatus.Code==AdvertisementStatusCodes.Published&&x.AdvertisementCategory.IsActive&&x.Province.IsActive&&x.City.IsActive).OrderByDescending(x=>x.PublishDate).Take(Math.Clamp(count,1,30));var items=await ProjectPublic(q).ToListAsync();SetPersianDates(items);return items;}

    public async Task<CustomerAdvertisementListComplexModel> SearchCustomerAsync(string userId,AdvertisementSearchModel model)
    {
        var q=repository.Query().Where(x=>x.UserID==userId);
        if(!string.IsNullOrWhiteSpace(model.Keyword)){var pattern=$"%{model.Keyword.Trim()}%";q=q.Where(x=>EF.Functions.Like(x.Title,pattern)||EF.Functions.Like(x.Description,pattern));}
        if(model.AdvertisementCategoryID.HasValue)q=q.Where(x=>x.AdvertisementCategoryID==model.AdvertisementCategoryID.Value);if(model.ProvinceID.HasValue)q=q.Where(x=>x.ProvinceID==model.ProvinceID.Value);if(model.CityID.HasValue)q=q.Where(x=>x.CityID==model.CityID.Value);if(model.IsImmediate.HasValue)q=q.Where(x=>x.IsImmediate==model.IsImmediate.Value);q=Sort(q,model.Sort);
        var projected=q.Select(x=>new CustomerAdvertisementListItem{AdvertisementID=x.AdvertisementID,Title=x.Title,Price=x.Price,IsImmediate=x.IsImmediate,IsFeatured=x.IsFeatured,CategoryName=x.AdvertisementCategory.CategoryName,CategorySlug=x.AdvertisementCategory.Slug,ProvinceName=x.Province.ProvinceName,CityName=x.City.CityName,ThumbnailPath=x.Images.OrderByDescending(i=>i.IsMainImage).ThenBy(i=>i.DisplayOrder).Select(i=>i.ThumbnailPath).FirstOrDefault(),CreateDate=x.CreateDate,CreateDatePersian="",Slug=x.Slug,AdvertisementStatusID=x.AdvertisementStatusID,StatusCode=x.AdvertisementStatus.Code,StatusTitle=x.AdvertisementStatus.Title,RejectionReason=x.RejectionReason});
        var items=await pagination.PaginateAsync(projected,model);SetPersianDates(items);return new(){Items=items,PageModel=model};
    }

    private async Task<CustomerAdvertisementDetailsModel?> GetPrivateDetailsAsync(IQueryable<DomainModel.Models.Advertisement> q)
    {
        var model=await q.Select(x=>new CustomerAdvertisementDetailsModel{AdvertisementID=x.AdvertisementID,Title=x.Title,Description=x.Description,Price=x.Price,IsImmediate=x.IsImmediate,AdvertisementCategoryID=x.AdvertisementCategoryID,CategoryName=x.AdvertisementCategory.CategoryName,CategorySlug=x.AdvertisementCategory.Slug,ProvinceID=x.ProvinceID,ProvinceName=x.Province.ProvinceName,CityID=x.CityID,CityName=x.City.CityName,PhoneNumber=x.PhoneNumber,CreateDate=x.CreateDate,UpdateDate=x.UpdateDate,PublishDate=x.PublishDate,ViewCount=x.ViewCount,Slug=x.Slug,SeoTitle=x.SeoTitle,SeoDescription=x.SeoDescription,SeoKeywords=x.SeoKeywords,CanonicalUrl=x.CanonicalUrl,OpenGraphImageUrl=x.OpenGraphImageUrl,IsIndexable=x.IsIndexable,IsFollow=x.IsFollow,AdvertisementStatusID=x.AdvertisementStatusID,StatusCode=x.AdvertisementStatus.Code,StatusTitle=x.AdvertisementStatus.Title,RejectionReason=x.RejectionReason,Images=x.Images.OrderByDescending(i=>i.IsMainImage).ThenBy(i=>i.DisplayOrder).Select(i=>new AdvertisementImageModel{AdvertisementImageID=i.AdvertisementImageID,ImagePath=i.ImagePath,ThumbnailPath=i.ThumbnailPath,Title=i.Title,AltText=i.AltText,IsMainImage=i.IsMainImage,DisplayOrder=i.DisplayOrder}).ToList()}).FirstOrDefaultAsync();
        if(model is not null){model.CreateDatePersian=model.CreateDate.ToPersianDateTime();model.UpdateDatePersian=model.UpdateDate.ToPersianDateTime();model.PublishDatePersian=model.PublishDate.ToPersianDateTime();model.Breadcrumb=await breadcrumb.BuildAdvertisementAsync(model.AdvertisementCategoryID,model.Title);}return model;
    }
    public Task<CustomerAdvertisementDetailsModel?> GetCustomerDetailsAsync(string userId,long id)=>GetPrivateDetailsAsync(repository.Query().Where(x=>x.AdvertisementID==id&&x.UserID==userId));
    public Task<CustomerAdvertisementDetailsModel?> GetAdminDetailsAsync(long id)=>GetPrivateDetailsAsync(repository.Query().Where(x=>x.AdvertisementID==id));

    private async Task<string?> ValidateLocationAndCategoryAsync(int categoryId,int provinceId,int cityId)
    {
        var category=await repository.GetCategoryAsync(categoryId);if(category is null||!category.IsActive||category.Depth!=2)return "دسته‌بندی انتخاب‌شده معتبر نیست. آگهی باید در دسته‌بندی سطح دوم ثبت شود.";
        var province=await repository.GetProvinceAsync(provinceId);if(province is null||!province.IsActive)return "استان انتخاب‌شده معتبر نیست.";
        var city=await repository.GetCityAsync(cityId);if(city is null||!city.IsActive||city.ProvinceID!=provinceId)return "شهر انتخاب‌شده متعلق به استان انتخاب‌شده نیست یا غیرفعال است.";
        return null;
    }
    private async Task<string> UniqueSlugAsync(string title,long? ignoreId=null)
    {
        var baseSlug=SeoHelper.ToSlug(title)??"advertisement";var candidate=baseSlug;var i=2;
        while(await repository.Query().AnyAsync(x=>x.Slug==candidate&&(!ignoreId.HasValue||x.AdvertisementID!=ignoreId.Value))){candidate=$"{baseSlug}-{i}";i++;}
        return candidate;
    }

    public async Task<OperationResult> AddAsync(string userId,AdvertisementAddModel model)
    {
        var result=new OperationResult("ثبت آگهی");var error=await ValidateLocationAndCategoryAsync(model.AdvertisementCategoryID,model.ProvinceID,model.CityID);if(error is not null)return result.ToFailed(error);
        var membership=await repository.GetActiveMembershipAsync(userId);if(membership is null||!membership.IsActive||membership.StartDate>DateTime.Now||membership.EndDate<DateTime.Now||!membership.MembershipPlan.IsActive)return result.ToFailed("برای ثبت آگهی باید یک عضویت فعال داشته باشید.");
        var count=await repository.CountAdvertisementsForMembershipAsync(membership.UserMembershipID);if(count>=membership.MembershipPlan.AdvertisementLimit)return result.ToFailed("تعداد آگهی‌های مجاز این عضویت به پایان رسیده است.");
        var draft=await repository.GetStatusByCodeAsync(AdvertisementStatusCodes.Draft);if(draft is null)return result.ToFailed("وضعیت پیش‌نویس در سیستم تعریف نشده است.");
        var entity=AdvertisementMapper.ToEntity(model,userId,membership.UserMembershipID,draft.AdvertisementStatusID);entity.Slug=await UniqueSlugAsync(model.Title);await repository.AddAsync(entity);await repository.SaveChangesAsync();return result.ToSuccess("آگهی به صورت پیش‌نویس ثبت شد.",entity.AdvertisementID);
    }

    public async Task<OperationResult> UpdateAsync(string userId,long id,AdvertisementEditModel model)
    {
        var result=new OperationResult("ویرایش آگهی");var entity=await repository.GetOwnedByIdAsync(id,userId);if(entity is null)return result.ToFailed("آگهی پیدا نشد یا متعلق به شما نیست.",id);
        var code=await repository.Query().Where(x=>x.AdvertisementID==id).Select(x=>x.AdvertisementStatus.Code).FirstAsync();if(code!=AdvertisementStatusCodes.Draft&&code!=AdvertisementStatusCodes.Rejected)return result.ToFailed("فقط آگهی پیش‌نویس یا ردشده قابل ویرایش است.",id);
        var error=await ValidateLocationAndCategoryAsync(model.AdvertisementCategoryID,model.ProvinceID,model.CityID);if(error is not null)return result.ToFailed(error,id);
        AdvertisementMapper.MapForUpdate(model,entity);entity.Slug=await UniqueSlugAsync(model.Title,id);await repository.SaveChangesAsync();return result.ToSuccess("آگهی ویرایش شد.",id);
    }

    public async Task<OperationResult> DeleteAsync(string userId,long id)
    {
        var result=new OperationResult("حذف آگهی");var entity=await repository.GetOwnedByIdAsync(id,userId);if(entity is null)return result.ToFailed("آگهی پیدا نشد یا متعلق به شما نیست.",id);var categoryId=entity.AdvertisementCategoryID;var wasPublished=await repository.Query().AnyAsync(x=>x.AdvertisementID==id&&x.AdvertisementStatus.Code==AdvertisementStatusCodes.Published);repository.Remove(entity);await repository.SaveChangesAsync();if(wasPublished){await repository.RefreshCategoryCountsAsync(categoryId);await repository.SaveChangesAsync();}return result.ToSuccess("آگهی حذف شد.",id);
    }

    public async Task<OperationResult> SubmitAsync(string userId,long id)
    {
        var r=new OperationResult("ارسال برای بررسی");var e=await repository.GetOwnedByIdAsync(id,userId);if(e is null)return r.ToFailed("آگهی پیدا نشد یا متعلق به شما نیست.",id);var current=await repository.Query().Where(x=>x.AdvertisementID==id).Select(x=>x.AdvertisementStatus.Code).FirstAsync();if(current!=AdvertisementStatusCodes.Draft&&current!=AdvertisementStatusCodes.Rejected)return r.ToFailed("این آگهی در وضعیت قابل ارسال نیست.",id);var pending=await repository.GetStatusByCodeAsync(AdvertisementStatusCodes.Pending);if(pending is null)return r.ToFailed("وضعیت در انتظار بررسی تعریف نشده است.");e.AdvertisementStatusID=pending.AdvertisementStatusID;e.RejectionReason=null;e.ReviewedByUserID=null;e.ReviewedDate=null;e.UpdateDate=DateTime.Now;await repository.SaveChangesAsync();return r.ToSuccess("آگهی برای بررسی ارسال شد.",id);
    }

    public async Task<AdminAdvertisementListComplexModel> SearchAdminAsync(AdminAdvertisementSearchModel m)
    {
        var q=repository.Query();if(!string.IsNullOrWhiteSpace(m.Keyword)){var pattern=$"%{m.Keyword.Trim()}%";q=q.Where(x=>EF.Functions.Like(x.Title,pattern)||EF.Functions.Like(x.Description,pattern));}if(m.AdvertisementStatusID.HasValue)q=q.Where(x=>x.AdvertisementStatusID==m.AdvertisementStatusID);if(m.AdvertisementCategoryID.HasValue)q=q.Where(x=>x.AdvertisementCategoryID==m.AdvertisementCategoryID);if(m.ProvinceID.HasValue)q=q.Where(x=>x.ProvinceID==m.ProvinceID);if(m.CityID.HasValue)q=q.Where(x=>x.CityID==m.CityID);if(!string.IsNullOrWhiteSpace(m.CustomerUserID))q=q.Where(x=>x.UserID==m.CustomerUserID);if(m.IsImmediate.HasValue)q=q.Where(x=>x.IsImmediate==m.IsImmediate);if(m.IsFeatured.HasValue)q=q.Where(x=>x.IsFeatured==m.IsFeatured);if(m.FromDate.HasValue)q=q.Where(x=>x.CreateDate>=m.FromDate.Value);if(m.ToDate.HasValue)q=q.Where(x=>x.CreateDate<m.ToDate.Value.Date.AddDays(1));q=Sort(q,m.Sort);
        var p=q.Select(x=>new AdminAdvertisementListItem{AdvertisementID=x.AdvertisementID,Title=x.Title,Price=x.Price,IsImmediate=x.IsImmediate,IsFeatured=x.IsFeatured,CategoryName=x.AdvertisementCategory.CategoryName,CategorySlug=x.AdvertisementCategory.Slug,ProvinceName=x.Province.ProvinceName,CityName=x.City.CityName,ThumbnailPath=x.Images.OrderByDescending(i=>i.IsMainImage).ThenBy(i=>i.DisplayOrder).Select(i=>i.ThumbnailPath).FirstOrDefault(),CreateDate=x.CreateDate,CreateDatePersian="",Slug=x.Slug,AdvertisementStatusID=x.AdvertisementStatusID,StatusCode=x.AdvertisementStatus.Code,StatusTitle=x.AdvertisementStatus.Title,RejectionReason=x.RejectionReason,UserID=x.UserID,CustomerName=x.User.FirstName+" "+x.User.LastName,CustomerMobileNumber=x.User.PhoneNumber??string.Empty});var items=await pagination.PaginateAsync(p,m);SetPersianDates(items);return new(){Items=items,PageModel=m};
    }

    public async Task<OperationResult> ApproveAsync(string adminUserId,long id)
    {
        var r=new OperationResult("تأیید آگهی");var e=await repository.GetByIdAsync(id);if(e is null)return r.ToFailed("آگهی پیدا نشد.",id);var current=await repository.Query().Where(x=>x.AdvertisementID==id).Select(x=>x.AdvertisementStatus.Code).FirstAsync();if(current!=AdvertisementStatusCodes.Pending)return r.ToFailed("فقط آگهی در انتظار بررسی قابل تأیید است.",id);var published=await repository.GetStatusByCodeAsync(AdvertisementStatusCodes.Published);if(published is null)return r.ToFailed("وضعیت منتشرشده تعریف نشده است.");e.AdvertisementStatusID=published.AdvertisementStatusID;e.PublishDate=DateTime.Now;e.ReviewedByUserID=adminUserId;e.ReviewedDate=DateTime.Now;e.RejectionReason=null;await repository.SaveChangesAsync();await repository.RefreshCategoryCountsAsync(e.AdvertisementCategoryID);await repository.SaveChangesAsync();return r.ToSuccess("آگهی منتشر شد.",id);
    }
    public async Task<OperationResult> RejectAsync(string adminUserId,long id,AdvertisementRejectModel model){var r=new OperationResult("رد آگهی");var e=await repository.GetByIdAsync(id);if(e is null)return r.ToFailed("آگهی پیدا نشد.",id);var current=await repository.Query().Where(x=>x.AdvertisementID==id).Select(x=>x.AdvertisementStatus.Code).FirstAsync();if(current!=AdvertisementStatusCodes.Pending)return r.ToFailed("فقط آگهی در انتظار بررسی قابل رد است.",id);var status=await repository.GetStatusByCodeAsync(AdvertisementStatusCodes.Rejected);if(status is null)return r.ToFailed("وضعیت ردشده تعریف نشده است.");e.AdvertisementStatusID=status.AdvertisementStatusID;e.RejectionReason=model.RejectionReason.Trim();e.ReviewedByUserID=adminUserId;e.ReviewedDate=DateTime.Now;await repository.SaveChangesAsync();return r.ToSuccess("آگهی رد شد.",id);}
    public async Task<OperationResult> DisableAsync(string adminUserId,long id){var r=new OperationResult("غیرفعال‌سازی آگهی");var e=await repository.GetByIdAsync(id);if(e is null)return r.ToFailed("آگهی پیدا نشد.",id);var current=await repository.Query().Where(x=>x.AdvertisementID==id).Select(x=>x.AdvertisementStatus.Code).FirstAsync();var wasPublished=current==AdvertisementStatusCodes.Published;if(current!=AdvertisementStatusCodes.Published&&current!=AdvertisementStatusCodes.Pending)return r.ToFailed("این آگهی در وضعیت قابل غیرفعال‌سازی نیست.",id);var status=await repository.GetStatusByCodeAsync(AdvertisementStatusCodes.Disabled);if(status is null)return r.ToFailed("وضعیت غیرفعال تعریف نشده است.");e.AdvertisementStatusID=status.AdvertisementStatusID;e.ReviewedByUserID=adminUserId;e.ReviewedDate=DateTime.Now;await repository.SaveChangesAsync();if(wasPublished){await repository.RefreshCategoryCountsAsync(e.AdvertisementCategoryID);await repository.SaveChangesAsync();}return r.ToSuccess("آگهی غیرفعال شد.",id);}
    public async Task<OperationResult> SetFeaturedAsync(long id,bool featured){var r=new OperationResult(featured?"ویترین آگهی":"حذف از ویترین");var e=await repository.GetByIdAsync(id);if(e is null)return r.ToFailed("آگهی پیدا نشد.",id);e.IsFeatured=featured;e.UpdateDate=DateTime.Now;await repository.SaveChangesAsync();return r.ToSuccess(featured?"آگهی به ویترین اضافه شد.":"آگهی از ویترین حذف شد.",id);}

    public async Task<AdvertisementListComplexModel> GetFavoritesAsync(string userId,AdvertisementSearchModel m)
    {
        var q=repository.FavoriteQuery().Where(f=>f.UserID==userId&&f.Advertisement.AdvertisementStatus.Code==AdvertisementStatusCodes.Published).OrderByDescending(f=>f.CreateDate).Select(f=>new AdvertisementListItem{AdvertisementID=f.AdvertisementID,Title=f.Advertisement.Title,Price=f.Advertisement.Price,IsImmediate=f.Advertisement.IsImmediate,IsFeatured=f.Advertisement.IsFeatured,CategoryName=f.Advertisement.AdvertisementCategory.CategoryName,CategorySlug=f.Advertisement.AdvertisementCategory.Slug,ProvinceName=f.Advertisement.Province.ProvinceName,CityName=f.Advertisement.City.CityName,ThumbnailPath=f.Advertisement.Images.OrderByDescending(i=>i.IsMainImage).ThenBy(i=>i.DisplayOrder).Select(i=>i.ThumbnailPath).FirstOrDefault(),CreateDate=f.Advertisement.CreateDate,CreateDatePersian="",Slug=f.Advertisement.Slug});var items=await pagination.PaginateAsync(q,m);SetPersianDates(items);return new(){Items=items,PageModel=m};
    }
    public async Task<OperationResult> AddFavoriteAsync(string userId,long advertisementId){var r=new OperationResult("ذخیره آگهی");if(!await repository.Query().AnyAsync(x=>x.AdvertisementID==advertisementId&&x.AdvertisementStatus.Code==AdvertisementStatusCodes.Published))return r.ToFailed("آگهی منتشرشده پیدا نشد.");if(await repository.GetFavoriteAsync(userId,advertisementId) is not null)return r.ToSuccess("این آگهی قبلاً ذخیره شده است.");var f=new FavoriteAdvertisement{UserID=userId,AdvertisementID=advertisementId,CreateDate=DateTime.Now};await repository.AddFavoriteAsync(f);await repository.SaveChangesAsync();return r.ToSuccess("آگهی ذخیره شد.",f.FavoriteAdvertisementID);}
    public async Task<OperationResult> RemoveFavoriteAsync(string userId,long advertisementId){var r=new OperationResult("حذف از ذخیره‌ها");var f=await repository.GetFavoriteAsync(userId,advertisementId);if(f is null)return r.ToFailed("این آگهی در ذخیره‌های شما نیست.");repository.RemoveFavorite(f);await repository.SaveChangesAsync();return r.ToSuccess("آگهی از ذخیره‌ها حذف شد.",f.FavoriteAdvertisementID);}

    public async Task<bool> CanManageAsync(string userId,long advertisementId,bool isAdmin=false)=>isAdmin?await repository.Query().AnyAsync(x=>x.AdvertisementID==advertisementId):await repository.Query().AnyAsync(x=>x.AdvertisementID==advertisementId&&x.UserID==userId);
    public async Task<List<AdvertisementImageModel>?> GetImagesForManagementAsync(string userId,long advertisementId,bool isAdmin=false){if(!await CanManageAsync(userId,advertisementId,isAdmin))return null;return await repository.Query().Where(x=>x.AdvertisementID==advertisementId).SelectMany(x=>x.Images).OrderByDescending(x=>x.IsMainImage).ThenBy(x=>x.DisplayOrder).Select(i=>new AdvertisementImageModel{AdvertisementImageID=i.AdvertisementImageID,ImagePath=i.ImagePath,ThumbnailPath=i.ThumbnailPath,Title=i.Title,AltText=i.AltText,IsMainImage=i.IsMainImage,DisplayOrder=i.DisplayOrder}).ToListAsync();}
    public async Task<AdvertisementImageModel?> GetImageForManagementAsync(string userId,long advertisementId,long imageId,bool isAdmin=false){if(!await CanManageAsync(userId,advertisementId,isAdmin))return null;return await repository.Query().Where(x=>x.AdvertisementID==advertisementId).SelectMany(x=>x.Images).Where(x=>x.AdvertisementImageID==imageId).Select(i=>new AdvertisementImageModel{AdvertisementImageID=i.AdvertisementImageID,ImagePath=i.ImagePath,ThumbnailPath=i.ThumbnailPath,Title=i.Title,AltText=i.AltText,IsMainImage=i.IsMainImage,DisplayOrder=i.DisplayOrder}).FirstOrDefaultAsync();}
    public async Task<OperationResult> AddImageMetadataAsync(string userId,long advertisementId,string imageName,string imagePath,string thumbnailPath,string? title,string? altText,bool isMain,bool isAdmin=false){var r=new OperationResult("افزودن تصویر");if(!await CanManageAsync(userId,advertisementId,isAdmin))return r.ToFailed("آگهی پیدا نشد یا دسترسی ندارید.");var existing=await repository.GetImagesAsync(advertisementId);if(existing.Count==0)isMain=true;if(isMain)foreach(var x in existing)x.IsMainImage=false;var img=new AdvertisementImage{AdvertisementID=advertisementId,ImageName=imageName,ImagePath=imagePath,ThumbnailPath=thumbnailPath,Title=title,AltText=altText,IsMainImage=isMain,DisplayOrder=existing.Count+1,CreateDate=DateTime.Now};await repository.AddImageAsync(img);await repository.SaveChangesAsync();return r.ToSuccess("تصویر اضافه شد.",img.AdvertisementImageID);}
    public async Task<OperationResult> DeleteImageMetadataAsync(string userId,long advertisementId,long imageId,bool isAdmin=false){var r=new OperationResult("حذف تصویر");if(!await CanManageAsync(userId,advertisementId,isAdmin))return r.ToFailed("آگهی پیدا نشد یا دسترسی ندارید.");var img=await repository.GetImageAsync(advertisementId,imageId);if(img is null)return r.ToFailed("تصویر پیدا نشد.",imageId);var wasMain=img.IsMainImage;repository.RemoveImage(img);await repository.SaveChangesAsync();if(wasMain){var remaining=await repository.GetImagesAsync(advertisementId);var first=remaining.FirstOrDefault();if(first is not null){first.IsMainImage=true;await repository.SaveChangesAsync();}}return r.ToSuccess("تصویر حذف شد.",imageId);}
    public async Task<OperationResult> SetMainImageAsync(string userId,long advertisementId,long imageId,bool isAdmin=false){var r=new OperationResult("تصویر اصلی");if(!await CanManageAsync(userId,advertisementId,isAdmin))return r.ToFailed("آگهی پیدا نشد یا دسترسی ندارید.");var images=await repository.GetImagesAsync(advertisementId);var target=images.FirstOrDefault(x=>x.AdvertisementImageID==imageId);if(target is null)return r.ToFailed("تصویر پیدا نشد.",imageId);foreach(var image in images)image.IsMainImage=image.AdvertisementImageID==imageId;await repository.SaveChangesAsync();return r.ToSuccess("تصویر اصلی تغییر کرد.",imageId);}
}
