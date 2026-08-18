using System.Text;
using Api.Authentication;
using Api.FileManager;
using Api.Middleware;
using Api.Seed;
using DataAccess.Repositories.Admin;
using DataAccess.Repositories.Advertisement;
using DataAccess.Repositories.AdvertisementCategory;
using DataAccess.Repositories.City;
using DataAccess.Repositories.HeroBanner;
using DataAccess.Repositories.Membership;
using DataAccess.Repositories.Payment;
using DataAccess.Repositories.Province;
using DataAccess.Repositories.SiteSetting;
using DataAccess.Services.Account;
using DataAccess.Services.Admin;
using DataAccess.Services.Advertisement;
using DataAccess.Services.AdvertisementCategory;
using DataAccess.Services.City;
using DataAccess.Services.Common;
using DataAccess.Services.HeroBanner;
using DataAccess.Services.Membership;
using DataAccess.Services.Payment;
using DataAccess.Services.Province;
using DataAccess.Services.SiteSetting;
using DomainModel.Context;
using DomainModel.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
          ?? throw new InvalidOperationException("تنظیمات JWT پیدا نشد.");
if (jwt.SigningKey.Length < 32)
    throw new InvalidOperationException("Jwt:SigningKey باید حداقل 32 کاراکتر باشد.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Classified Ads Educational API",
        Version = "v1",
        Description = "Persian educational classified advertisement API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Repositories
builder.Services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
builder.Services.AddScoped<IAdvertisementCategoryRepository, AdvertisementCategoryRepository>();
builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ISiteSettingRepository, SiteSettingRepository>();
builder.Services.AddScoped<IHeroBannerRepository, HeroBannerRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

// Services
builder.Services.AddScoped<IPaginationService, PaginationService>();
builder.Services.AddScoped<IBreadcrumbService, BreadcrumbService>();
builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
builder.Services.AddScoped<IAdvertisementCategoryService, AdvertisementCategoryService>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ISiteSettingService, SiteSettingService>();
builder.Services.AddScoped<IHeroBannerService, HeroBannerService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IFileManager, FileManager>();
builder.Services.AddScoped<DataSeeder>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("Frontend");
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (app.Configuration.GetValue("Database:InitializeWithEnsureCreated", true))
        await db.Database.EnsureCreatedAsync();
    else
        await db.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

app.Run();
