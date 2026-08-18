# Classified Ads Educational API

A Persian ASP.NET Core classified-advertisement API generated from the finalized course specification.

## What is implemented

- 4 projects only: `Framework`, `DomainModel`, `DataAccess`, `Api`
- ASP.NET Core Identity + JWT (`Admin`, `Customer`)
- Mobile number as username
- Register / login / logout / authenticated user / change password
- Advertisement CRUD, ownership, Draft → Pending → Published/Rejected workflow
- `IsImmediate` and Admin-controlled `IsFeatured`
- 2-level categories with `ParentID`, `Depth`, `Lineage`, `SortOrder`, `AdvertisementCount`
- Stable public route: `GET /api/categories/menu`
- Province → City endpoints
- Membership plans and `UserMembership`
- **No `UsedAdvertisements` field**: the advertisement limit is calculated from Ads linked by `UserMembershipID`
- One-click simulated membership payment
- Favorites
- Advertisement image upload + thumbnail + main image
- Customer avatar upload
- Physical image deletion when an advertisement/image is deleted
- Public homepage aggregate endpoint
- Site settings and hero banners
- One `CustomerController` and one `AdminController`
- Admin dashboard, customer management, ad moderation, location/category/membership CRUD, payment report
- Exact requested `PageModel` and `OperationResult`
- Persian DataAnnotations and Persian date display strings
- SQL-side filtering, sorting, projection and pagination

## Requirements

- .NET 10 SDK (10.0.302 or a compatible .NET 10 SDK)
- SQL Server / SQL Server LocalDB
- Internet access on first `dotnet restore`

The default connection string uses SQL Server LocalDB and is convenient on Windows with Visual Studio:

```text
Server=(localdb)\MSSQLLocalDB;Database=ClassifiedAdsEducationalDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

If you use SQL Server Express/full SQL Server, edit `Api/appsettings.json` before running.

## Fastest first run

From the solution root:

```bash
dotnet restore
dotnet run --project Api --launch-profile http
```

Open:

```text
http://localhost:5086/swagger
```

For the downloadable educational build, `Database:InitializeWithEnsureCreated` is `true`. On the first run the database schema is created directly and demo data is seeded. This makes first testing easy without needing a migration first.

## Seeded logins

### Admin

```text
Mobile:   09120000001
Password: Admin@123456
Role:     Admin
```

### Main Customer

```text
Mobile:   09120000002
Password: Customer@123456
Role:     Customer
```

The main Customer starts without a membership so you can test the simulated purchase flow.

### Demo Customers

```text
09120000101 ... 09120000110
Password: Demo@123456
Role: Customer
```

Each demo Customer receives an active membership and three Published advertisements (30 demo advertisements total).

> Change all development credentials and the JWT signing key before any real deployment.

## Recommended Swagger test sequence

1. `GET /api/site/home`
2. `GET /api/categories/menu`
3. `GET /api/provinces`
4. `GET /api/advertisements`
5. Login as main Customer with `/api/account/login`
6. Click **Authorize** in Swagger and paste the returned JWT
7. `GET /api/customer/membership` → initially empty/null
8. `GET /api/membership-plans`
9. `POST /api/customer/membership/purchase/{membershipPlanId}` → simulated successful payment + membership
10. `GET /api/customer/membership`
11. Create a Draft advertisement with a **Depth-2** category and a City that belongs to the selected Province
12. Upload images through FileManager routes
13. Submit the ad
14. Login as Admin and approve the ad
15. Confirm the ad appears in the public search
16. Test Favorites and Admin payment reports

A ready-to-edit request file is included at `Api/ClassifiedAdsApi.http`.

## Simulated payment

There is no real payment gateway. The Customer clicks a membership purchase button and the frontend calls:

```http
POST /api/customer/membership/purchase/{membershipPlanId}
```

The API immediately:

1. validates the plan,
2. creates a paid `Payment`,
3. generates a `SIM-...` tracking code,
4. closes previous active memberships,
5. creates a new active `UserMembership`,
6. returns `OperationResult`.

Payment data is only for educational Customer history, Admin reports and dashboard totals.

## Advertisement limit rule

`UserMembership` intentionally does **not** have a `UsedAdvertisements` counter.

When a Customer creates a new advertisement, the API runs the equivalent of:

```csharp
await context.Advertisements.CountAsync(x =>
    x.UserMembershipID == membership.UserMembershipID);
```

and compares the result with `MembershipPlan.AdvertisementLimit`.

## Iran city seed

The seeder always creates all 31 Iranian provinces.

By default it then attempts to import the full city dataset from the open-source `sajaddp/list-of-cities-in-Iran` JSON data source, which describes 31 provinces and 1,659 cities based on Iranian administrative data through 1402/2023. If that online import is unavailable, the project falls back to an embedded practical set of major cities so the API can still start and be tested.

You can disable the online import:

```json
"SeedData": {
  "TryOnlineIranCities": false
}
```

The external location dataset is maintained under GPL-3.0 by its upstream project. It is fetched at development seed time rather than copied into this repository. Verify geographic data against your preferred official source before production use.

## EF Core migration workflow

The instant-test database uses `EnsureCreated` by default. **Do not create a first migration against an existing database that was already created with `EnsureCreated`.**

To switch to the normal migration workflow:

1. Stop the API.
2. Delete `ClassifiedAdsEducationalDb`.
3. Install EF CLI if needed:

```bash
dotnet tool install --global dotnet-ef --version 10.0.10
```

4. Create the migration:

```bash
dotnet ef migrations add InitialCreate --project DomainModel --startup-project Api
```

5. Change:

```json
"Database": {
  "InitializeWithEnsureCreated": false
}
```

6. Apply:

```bash
dotnet ef database update --project DomainModel --startup-project Api
```

After that the app uses `Database.MigrateAsync()` at startup.

## Important public routes

```text
GET  /api/site/home
GET  /api/site/settings
GET  /api/categories/menu
GET  /api/categories/{id}
GET  /api/categories/by-slug/{slug}
GET  /api/provinces
GET  /api/cities/by-province/{provinceId}
GET  /api/advertisements
GET  /api/advertisements/{id}
GET  /api/advertisements/by-slug/{slug}
GET  /api/membership-plans
```

Account:

```text
POST /api/account/register
POST /api/account/login
POST /api/account/logout
GET  /api/account/authenticated-user
POST /api/account/change-password
```

Customer:

```text
GET    /api/customer/advertisements
GET    /api/customer/advertisements/{id}
POST   /api/customer/advertisements
PUT    /api/customer/advertisements/{id}
DELETE /api/customer/advertisements/{id}
POST   /api/customer/advertisements/{id}/submit
GET    /api/customer/favorites
POST   /api/customer/favorites/{advertisementId}
DELETE /api/customer/favorites/{advertisementId}
GET    /api/customer/membership
POST   /api/customer/membership/purchase/{membershipPlanId}
GET    /api/customer/payments
```

Files:

```text
POST   /api/file-manager/advertisements/{advertisementId}/images
DELETE /api/file-manager/advertisements/{advertisementId}/images/{imageId}
PATCH  /api/file-manager/advertisements/{advertisementId}/images/{imageId}/main
POST   /api/file-manager/customer/avatar
POST   /api/file-manager/site/logo
POST   /api/file-manager/site/favicon
POST   /api/file-manager/hero-banners/{id}/image
```

Admin routes are grouped under `/api/admin/...` in one `AdminController`.

## Frontend integration notes

- `GET /api/categories/menu` already returns a 2-level tree; JavaScript does not need to rebuild category hierarchy.
- Paginated list responses use `Items + PageModel`.
- Province and City are loaded separately with `GET /api/cities/by-province/{provinceId}`.
- `IsFeatured` is Admin-only; Customer Add/Edit models only expose `IsImmediate`.
- `AuthenticatedUserModel` returns `AvatarPath` for the navbar.
- `GET /api/site/home` combines SiteSetting, HeroBanners, Categories, FeaturedAdvertisements and LatestAdvertisements.
- JWT storage is a frontend concern: use localStorage only when RememberMe is checked, otherwise sessionStorage.

## Generation validation note

The source was generated and statically checked in an environment where the .NET SDK executable was not available. Because of that, I could not run `dotnet restore/build` inside the generation environment. The package includes a validation report and run instructions; the first real compiler validation should be run on your machine with .NET 10 installed.
