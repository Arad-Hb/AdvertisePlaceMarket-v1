# Validation Summary

Validated against the uploaded final API package `ClassifiedAdsApi(1).zip`.

## API endpoint checks

The following Visitor routes were found in the API-generated endpoint metadata and matched by the frontend configuration:

- `GET /api/site/home`
- `GET /api/site/settings`
- `GET /api/categories/menu`
- `GET /api/provinces`
- `GET /api/cities/by-province/{provinceId}`
- `GET /api/advertisements`
- `GET /api/advertisements/{id}`
- `GET /api/advertisements/by-slug/{slug}`
- `GET /api/membership-plans`
- `POST /api/account/login`
- `POST /api/account/register`
- `POST /api/account/logout`
- `GET /api/account/authenticated-user`
- `POST /api/customer/favorites/{advertisementId}`
- `DELETE /api/customer/favorites/{advertisementId}`
- `POST /api/customer/membership/purchase/{membershipPlanId}`

The public Advertisement endpoint exposes the exact query parameters used by the frontend:

`Keyword`, `AdvertisementCategoryID`, `ProvinceID`, `CityID`, `MinPrice`, `MaxPrice`, `IsImmediate`, `Sort`, `PageIndex`, `PageSize`.

## Static frontend checks

- All JavaScript files pass `node --check`.
- Every local HTML `src` and `href` asset reference exists.
- No functional `fetch(` call exists.
- No Visitor `IsFeatured`, `ویژه`, or `توافقی` implementation exists.
- No removed About/Terms/Privacy/Forgot Password page link remains.
- Login/register payload fields match `LoginModel` and `RegisterModel`.
- Authenticated-user fields match `AuthenticatedUserModel`.
- Pricing fields match `MembershipPlanListItem`.
- Advertisement card/detail fields match `AdvertisementListItem` / `AdvertisementDetailsModel`.

## Live-run limitation

This environment does not contain the .NET SDK or SQL Server, so the uploaded ASP.NET Core API cannot be booted here for a real database-backed browser integration test. The API source, generated `ApiEndpoints.json`, DTOs, controllers, and service sort/filter contracts were inspected directly. Run the included frontend against `http://localhost:5086` after starting the API on your Windows development machine for final end-to-end verification.
