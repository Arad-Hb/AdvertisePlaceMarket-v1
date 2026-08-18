using System.Text.Json;
using System.Text.Json.Serialization;
using DomainModel.Context;
using DomainModel.Models;
using Framework.Common.Constants;
using Framework.Common.Seo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Seed;

public class DataSeeder(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory)
{
    public const string AdminId = "11111111-1111-1111-1111-111111111111";
    public const string MainCustomerId = "22222222-2222-2222-2222-222222222222";

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedStatusesAsync();
        var admin = await SeedConfiguredUserAsync(
            AdminId,
            configuration["SeedUsers:Admin:MobileNumber"] ?? "09120000001",
            configuration["SeedUsers:Admin:Password"] ?? "Admin@123456",
            configuration["SeedUsers:Admin:FirstName"] ?? "مدیر",
            configuration["SeedUsers:Admin:LastName"] ?? "سیستم",
            "Admin");

        await SeedConfiguredUserAsync(
            MainCustomerId,
            configuration["SeedUsers:Customer:MobileNumber"] ?? "09120000002",
            configuration["SeedUsers:Customer:Password"] ?? "Customer@123456",
            configuration["SeedUsers:Customer:FirstName"] ?? "مشتری",
            configuration["SeedUsers:Customer:LastName"] ?? "نمونه",
            "Customer");

        await SeedProvincesAsync();
        await SeedCitiesAsync();
        await SeedCategoriesAsync();
        await SeedMembershipPlansAsync();
        var demoUsers = await SeedDemoUsersAsync();
        await SeedDemoMembershipsAndAdvertisementsAsync(demoUsers, admin.Id);
        await SeedSiteSettingAsync();
        await SeedHeroBannersAsync();
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in new[] { "Admin", "Customer" })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
    }

    private async Task<ApplicationUser> SeedConfiguredUserAsync(
        string id, string mobile, string password, string firstName, string lastName, string role)
    {
        var user = await userManager.FindByNameAsync(mobile);
        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = id,
                UserName = mobile,
                PhoneNumber = mobile,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                CreateDate = DateTime.Now
            };
            var created = await userManager.CreateAsync(user, password);
            if (!created.Succeeded)
                throw new InvalidOperationException(string.Join(" | ", created.Errors.Select(x => x.Description)));
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);
        return user;
    }

    private async Task SeedStatusesAsync()
    {
        if (await context.AdvertisementStatuses.AnyAsync()) return;
        context.AdvertisementStatuses.AddRange(
            new AdvertisementStatus { Code = AdvertisementStatusCodes.Draft, Title = "پیش‌نویس", SortOrder = 1 },
            new AdvertisementStatus { Code = AdvertisementStatusCodes.Pending, Title = "در انتظار بررسی", SortOrder = 2 },
            new AdvertisementStatus { Code = AdvertisementStatusCodes.Published, Title = "منتشرشده", SortOrder = 3 },
            new AdvertisementStatus { Code = AdvertisementStatusCodes.Rejected, Title = "ردشده", SortOrder = 4 },
            new AdvertisementStatus { Code = AdvertisementStatusCodes.Expired, Title = "منقضی", SortOrder = 5 },
            new AdvertisementStatus { Code = AdvertisementStatusCodes.Disabled, Title = "غیرفعال", SortOrder = 6 });
        await context.SaveChangesAsync();
    }

    private static readonly string[] ProvinceNames =
    {
        "آذربایجان شرقی", "آذربایجان غربی", "اردبیل", "اصفهان", "البرز", "ایلام", "بوشهر", "تهران",
        "چهارمحال و بختیاری", "خراسان جنوبی", "خراسان رضوی", "خراسان شمالی", "خوزستان", "زنجان", "سمنان",
        "سیستان و بلوچستان", "فارس", "قزوین", "قم", "کردستان", "کرمان", "کرمانشاه", "کهگیلویه و بویراحمد",
        "گلستان", "گیلان", "لرستان", "مازندران", "مرکزی", "هرمزگان", "همدان", "یزد"
    };

    private async Task SeedProvincesAsync()
    {
        foreach (var (name, index) in ProvinceNames.Select((name, index) => (name, index)))
        {
            if (await context.Provinces.AnyAsync(x => x.ProvinceName == name)) continue;
            context.Provinces.Add(new Province
            {
                ProvinceName = name,
                DisplayOrder = index + 1,
                IsActive = true,
                Slug = SeoHelper.ToSlug(name)
            });
        }
        await context.SaveChangesAsync();
    }

    private static readonly Dictionary<int, string> ExternalProvinceNames = new()
    {
        [100]="مرکزی", [101]="گیلان", [102]="مازندران", [103]="آذربایجان شرقی", [104]="آذربایجان غربی",
        [105]="کرمانشاه", [106]="خوزستان", [107]="فارس", [108]="کرمان", [109]="خراسان رضوی", [110]="اصفهان",
        [111]="سیستان و بلوچستان", [112]="کردستان", [113]="همدان", [114]="چهارمحال و بختیاری", [115]="لرستان",
        [116]="ایلام", [117]="کهگیلویه و بویراحمد", [118]="بوشهر", [119]="زنجان", [120]="سمنان", [121]="یزد",
        [122]="هرمزگان", [123]="تهران", [124]="اردبیل", [125]="قم", [126]="قزوین", [127]="گلستان",
        [128]="خراسان شمالی", [129]="خراسان جنوبی", [130]="البرز"
    };

    private async Task SeedCitiesAsync()
    {
        if (await context.Cities.AnyAsync()) return;

        var imported = false;
        if (configuration.GetValue("SeedData:TryOnlineIranCities", true))
        {
            try
            {
                var url = configuration["SeedData:IranCitiesUrl"]
                    ?? "https://raw.githubusercontent.com/sajaddp/list-of-cities-in-Iran/refs/heads/main/dist/json/cities.json";
                var client = httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(15);
                using var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                await using var stream = await response.Content.ReadAsStreamAsync();
                var sourceCities = await JsonSerializer.DeserializeAsync<List<ExternalCity>>(stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (sourceCities is { Count: > 0 })
                {
                    var provinces = await context.Provinces.AsNoTracking().ToDictionaryAsync(x => x.ProvinceName, x => x.ProvinceID);
                    var displayOrders = new Dictionary<int, int>();
                    foreach (var item in sourceCities)
                    {
                        if (!ExternalProvinceNames.TryGetValue(item.ProvinceId, out var provinceName)) continue;
                        if (!provinces.TryGetValue(provinceName, out var provinceId)) continue;
                        displayOrders[provinceId] = displayOrders.GetValueOrDefault(provinceId) + 1;
                        context.Cities.Add(new City
                        {
                            CityName = item.Name.Trim(),
                            ProvinceID = provinceId,
                            DisplayOrder = displayOrders[provinceId],
                            IsActive = true,
                            Slug = string.IsNullOrWhiteSpace(item.Slug) ? SeoHelper.ToSlug(item.Name) : item.Slug
                        });
                    }
                    await context.SaveChangesAsync();
                    imported = await context.Cities.AnyAsync();
                }
            }
            catch
            {
                foreach (var entry in context.ChangeTracker.Entries<City>()
                             .Where(x => x.State == EntityState.Added))
                {
                    entry.State = EntityState.Detached;
                }

                imported = false;
            }
        }

        if (!imported)
            await SeedFallbackCitiesAsync();
    }

    private async Task SeedFallbackCitiesAsync()
    {
        var fallback = new Dictionary<string, string[]>
        {
            ["آذربایجان شرقی"] = ["تبریز", "مراغه", "مرند", "اهر", "میانه"],
            ["آذربایجان غربی"] = ["ارومیه", "خوی", "مهاباد", "بوکان", "میاندوآب"],
            ["اردبیل"] = ["اردبیل", "مشگین شهر", "خلخال", "پارس آباد"],
            ["اصفهان"] = ["اصفهان", "کاشان", "خمینی شهر", "نجف آباد", "شاهین شهر"],
            ["البرز"] = ["کرج", "فردیس", "هشتگرد", "نظرآباد"],
            ["ایلام"] = ["ایلام", "دهلران", "آبدانان", "مهران"],
            ["بوشهر"] = ["بوشهر", "برازجان", "گناوه", "کنگان"],
            ["تهران"] = ["تهران", "شهریار", "اسلامشهر", "قدس", "ورامین", "پردیس"],
            ["چهارمحال و بختیاری"] = ["شهرکرد", "بروجن", "فارسان", "لردگان"],
            ["خراسان جنوبی"] = ["بیرجند", "قائن", "طبس", "فردوس"],
            ["خراسان رضوی"] = ["مشهد", "نیشابور", "سبزوار", "تربت حیدریه", "کاشمر"],
            ["خراسان شمالی"] = ["بجنورد", "شیروان", "اسفراین", "جاجرم"],
            ["خوزستان"] = ["اهواز", "آبادان", "خرمشهر", "دزفول", "ماهشهر", "اندیمشک"],
            ["زنجان"] = ["زنجان", "ابهر", "خرمدره", "قیدار"],
            ["سمنان"] = ["سمنان", "شاهرود", "دامغان", "گرمسار"],
            ["سیستان و بلوچستان"] = ["زاهدان", "چابهار", "ایرانشهر", "زابل", "سراوان"],
            ["فارس"] = ["شیراز", "مرودشت", "جهرم", "فسا", "لار", "کازرون"],
            ["قزوین"] = ["قزوین", "تاکستان", "آبیک", "الوند"],
            ["قم"] = ["قم"],
            ["کردستان"] = ["سنندج", "سقز", "مریوان", "بانه", "قروه"],
            ["کرمان"] = ["کرمان", "رفسنجان", "سیرجان", "جیرفت", "بم"],
            ["کرمانشاه"] = ["کرمانشاه", "اسلام آباد غرب", "جوانرود", "کنگاور"],
            ["کهگیلویه و بویراحمد"] = ["یاسوج", "دوگنبدان", "دهدشت"],
            ["گلستان"] = ["گرگان", "گنبد کاووس", "علی آباد", "بندر ترکمن"],
            ["گیلان"] = ["رشت", "بندر انزلی", "لاهیجان", "لنگرود", "آستارا"],
            ["لرستان"] = ["خرم آباد", "بروجرد", "دورود", "الیگودرز"],
            ["مازندران"] = ["ساری", "بابل", "آمل", "قائم شهر", "چالوس", "نوشهر"],
            ["مرکزی"] = ["اراک", "ساوه", "خمین", "محلات"],
            ["هرمزگان"] = ["بندرعباس", "قشم", "میناب", "بندر لنگه"],
            ["همدان"] = ["همدان", "ملایر", "نهاوند", "تویسرکان"],
            ["یزد"] = ["یزد", "میبد", "اردکان", "ابرکوه"]
        };

        var provinces = await context.Provinces.AsNoTracking().ToDictionaryAsync(x => x.ProvinceName, x => x.ProvinceID);
        foreach (var group in fallback)
        {
            if (!provinces.TryGetValue(group.Key, out var provinceId)) continue;
            for (var i = 0; i < group.Value.Length; i++)
                context.Cities.Add(new City
                {
                    CityName = group.Value[i],
                    ProvinceID = provinceId,
                    DisplayOrder = i + 1,
                    IsActive = true,
                    Slug = SeoHelper.ToSlug(group.Value[i])
                });
        }
        await context.SaveChangesAsync();
    }

    private async Task SeedCategoriesAsync()
    {
        if (await context.AdvertisementCategories.AnyAsync()) return;

        var tree = new Dictionary<string, string[]>
        {
            ["آموزش"] = ["آموزش تخصصی", "آموزش درسی", "آموزش فنی و حرفه ای", "آموزش هنری", "خدمات آموزشی"],
            ["استخدام"] = ["آرایشگر", "آشپز و مشاغل مرتبط", "بازاریاب", "برنامه نویس", "بسته بند", "پزشک و مشاغل مرتبط", "پیک", "تعمیرکار", "تکنسین", "حسابدار", "خیاط و مشاغل مرتبط", "راننده", "سایر مشاغل", "طراح", "فروشنده", "کارشناس", "کارگر ساده", "کارگر ماهر", "کارمند", "کافی من-کافی شاپ کار", "مدرس", "مدیر", "مربی", "مشاور", "منشی", "مهندس"],
            ["املاک"] = ["اجاره املاک اداری و تجاری", "اجاره املاک مسکونی", "خدمات املاک", "فروش املاک اداری و تجاری", "فروش املاک مسکونی"],
            ["پزشکی، زیبایی و بهداشتی"] = ["خدمات زیبایی", "درمان", "لوازم پزشکی و زیبایی", "محصولات آرایشی و بهداشتی"],
            ["چاپ و تبلیغات"] = ["تبلیغات", "چاپ"],
            ["حمل و نقل"] = ["حمل لوازم و اثاثه", "سرویس خصوصی"],
            ["خدمات بازرگانی"] = ["اخذ کارت بازرگانی", "انبارداری کالا", "ترخیص کالا", "خدمات بازرگانی - سایر", "طرح توجیهی - جواز تاسیس صنایع", "واردات - صادرات"],
            ["خدمات در منزل"] = ["امور مراقبتی", "امور منزل", "امور نظافتی"],
            ["خدمات ساختمانی"] = ["تاسیسات", "تزئینات", "ساختمان"],
            ["خدمات و لوازم صنعتی و صنفی"] = ["خدمات صنعتی و کشاورزی", "لوازم صنفی - فروشگاهی", "ماشین آلات صنعتی و کشاورزی", "مزایده - مناقصه"]
        };

        foreach (var parentName in tree.Keys)
            context.AdvertisementCategories.Add(new AdvertisementCategory
            {
                CategoryName = parentName,
                ParentID = null,
                Depth = 1,
                Lineage = "/pending/",
                SortOrder = 1,
                AdvertisementCount = 0,
                Slug = SeoHelper.ToSlug(parentName),
                IsActive = true,
                CreateDate = DateTime.Now
            });
        await context.SaveChangesAsync();

        var parents = await context.AdvertisementCategories.Where(x => x.Depth == 1).ToDictionaryAsync(x => x.CategoryName);
        foreach (var parent in parents.Values) parent.Lineage = $"/{parent.AdvertisementCategoryID}/";

        foreach (var entry in tree)
        {
            var parent = parents[entry.Key];
            foreach (var childName in entry.Value)
                context.AdvertisementCategories.Add(new AdvertisementCategory
                {
                    CategoryName = childName,
                    ParentID = parent.AdvertisementCategoryID,
                    Depth = 2,
                    Lineage = "/pending/",
                    SortOrder = 2,
                    AdvertisementCount = 0,
                    Slug = SeoHelper.ToSlug(childName),
                    IsActive = true,
                    CreateDate = DateTime.Now
                });
        }
        await context.SaveChangesAsync();

        var children = await context.AdvertisementCategories.Where(x => x.Depth == 2).ToListAsync();
        foreach (var child in children) child.Lineage = $"/{child.ParentID}/{child.AdvertisementCategoryID}/";
        await context.SaveChangesAsync();
    }

    private async Task SeedMembershipPlansAsync()
    {
        if (await context.MembershipPlans.AnyAsync()) return;
        context.MembershipPlans.AddRange(
            new MembershipPlan { Title = "پایه", Description = "پلن ساده برای تمرین", DurationDays = 30, AdvertisementLimit = 3, Price = 0, IsActive = true, SortOrder = 1 },
            new MembershipPlan { Title = "استاندارد", Description = "مناسب کاربران فعال", DurationDays = 60, AdvertisementLimit = 10, Price = 250000, IsActive = true, SortOrder = 2 },
            new MembershipPlan { Title = "حرفه‌ای", Description = "پلن نمونه با ظرفیت بیشتر", DurationDays = 90, AdvertisementLimit = 30, Price = 500000, IsActive = true, SortOrder = 3 });
        await context.SaveChangesAsync();
    }

    private async Task<List<ApplicationUser>> SeedDemoUsersAsync()
    {
        var users = new List<ApplicationUser>();
        var password = configuration["SeedUsers:DemoPassword"] ?? "Demo@123456";
        for (var i = 1; i <= 10; i++)
        {
            var mobile = $"091200001{i:00}";
            var id = $"33333333-3333-3333-3333-{i:000000000000}";
            users.Add(await SeedConfiguredUserAsync(id, mobile, password, $"کاربر {i}", "نمونه", "Customer"));
        }
        return users;
    }

    private async Task SeedDemoMembershipsAndAdvertisementsAsync(List<ApplicationUser> users, string adminId)
    {
        if (await context.Advertisements.AnyAsync()) return;

        var plan = await context.MembershipPlans.OrderBy(x => x.SortOrder).FirstAsync(x => x.AdvertisementLimit >= 3);
        var status = await context.AdvertisementStatuses.FirstAsync(x => x.Code == AdvertisementStatusCodes.Published);
        var categories = await context.AdvertisementCategories.Where(x => x.Depth == 2 && x.IsActive).OrderBy(x => x.AdvertisementCategoryID).ToListAsync();
        var cities = await context.Cities.Include(x => x.Province).Where(x => x.IsActive && x.Province.IsActive).OrderBy(x => x.CityID).Take(200).ToListAsync();
        if (categories.Count == 0 || cities.Count == 0) return;

        for (var i = 0; i < users.Count; i++)
        {
            var user = users[i];
            var membership = await context.UserMemberships.FirstOrDefaultAsync(x => x.UserID == user.Id && x.IsActive);
            if (membership is null)
            {
                membership = new UserMembership
                {
                    UserID = user.Id,
                    MembershipPlanID = plan.MembershipPlanID,
                    StartDate = DateTime.Now.AddDays(-5),
                    EndDate = DateTime.Now.AddDays(plan.DurationDays - 5),
                    PaidAmount = 0,
                    IsActive = true,
                    CreateDate = DateTime.Now.AddDays(-5)
                };
                context.UserMemberships.Add(membership);
                await context.SaveChangesAsync();
            }

            for (var j = 0; j < 3; j++)
            {
                var category = categories[(i * 3 + j) % categories.Count];
                var city = cities[(i * 7 + j * 3) % cities.Count];
                var title = $"آگهی نمونه {i + 1}-{j + 1} در {city.CityName}";
                context.Advertisements.Add(new Advertisement
                {
                    Title = title,
                    Description = "این آگهی به صورت خودکار برای تست و آموزش API ایجاد شده است.",
                    Price = (i + 1) * 1000000m + j * 250000m,
                    PhoneNumber = user.PhoneNumber ?? user.UserName ?? "09120000000",
                    AdvertisementCategoryID = category.AdvertisementCategoryID,
                    ProvinceID = city.ProvinceID,
                    CityID = city.CityID,
                    UserID = user.Id,
                    UserMembershipID = membership.UserMembershipID,
                    AdvertisementStatusID = status.AdvertisementStatusID,
                    IsImmediate = (i + j) % 3 == 0,
                    IsFeatured = i < 3 && j == 0,
                    ViewCount = (i + 1) * (j + 2) * 7,
                    CreateDate = DateTime.Now.AddDays(-(i + j + 1)),
                    PublishDate = DateTime.Now.AddDays(-(i + j + 1)),
                    ReviewedByUserID = adminId,
                    ReviewedDate = DateTime.Now.AddDays(-(i + j + 1)),
                    Slug = $"{SeoHelper.ToSlug(title)}-{i + 1}-{j + 1}"
                });
            }
        }
        await context.SaveChangesAsync();
        await RefreshCategoryCountsAsync();
    }

    private async Task RefreshCategoryCountsAsync()
    {
        var publishedId = await context.AdvertisementStatuses.Where(x => x.Code == AdvertisementStatusCodes.Published).Select(x => x.AdvertisementStatusID).FirstAsync();
        var categories = await context.AdvertisementCategories.ToListAsync();
        foreach (var child in categories.Where(x => x.Depth == 2))
            child.AdvertisementCount = await context.Advertisements.CountAsync(x => x.AdvertisementCategoryID == child.AdvertisementCategoryID && x.AdvertisementStatusID == publishedId);
        foreach (var parent in categories.Where(x => x.Depth == 1))
            parent.AdvertisementCount = categories.Where(x => x.ParentID == parent.AdvertisementCategoryID).Sum(x => x.AdvertisementCount);
        await context.SaveChangesAsync();
    }

    private async Task SeedSiteSettingAsync()
    {
        if (await context.SiteSettings.AnyAsync()) return;
        context.SiteSettings.Add(new SiteSetting
        {
            SiteName = "تحلیل داده آگهی",
            SiteDescription = "سامانه آموزشی ثبت و انتشار آگهی",
            FooterText = "پروژه آموزشی ASP.NET Core",
            IsSiteActive = true
        });
        await context.SaveChangesAsync();
    }

    private async Task SeedHeroBannersAsync()
    {
        if (await context.HeroBanners.AnyAsync()) return;
        context.HeroBanners.Add(new HeroBanner
        {
            Title = "آگهی خود را ساده ثبت کنید",
            Subtitle = "نمونه بنر قابل ویرایش از پنل مدیریت",
            ButtonText = "ثبت آگهی",
            LinkUrl = "/customer/advertisement-create.html",
            SortOrder = 1,
            IsActive = true,
            CreateDate = DateTime.Now
        });
        await context.SaveChangesAsync();
    }

    private sealed class ExternalCity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        [JsonPropertyName("province_id")]
        public int ProvinceId { get; set; }
    }
}
