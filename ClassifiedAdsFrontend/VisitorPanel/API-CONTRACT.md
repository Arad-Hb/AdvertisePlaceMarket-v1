# Visitor Frontend ↔ Final API Contract

This frontend was wired against the uploaded `ClassifiedAdsApi` source and its generated endpoint metadata.

## Runtime configuration

- API: `http://localhost:5086/api`
- Media/static backend files: `http://localhost:5086`
- Change both in `assets/js/config.js` when the API host changes.

## Public endpoints used

| Frontend feature | Method | API route |
|---|---|---|
| Home aggregate | GET | `/api/site/home` |
| Footer/site settings | GET | `/api/site/settings` |
| Two-level category menu | GET | `/api/categories/menu` |
| Category details | GET | `/api/categories/{id}` |
| Provinces | GET | `/api/provinces` |
| Cities after province | GET | `/api/cities/by-province/{provinceId}` |
| Advertisement search | GET | `/api/advertisements` |
| Advertisement detail | GET | `/api/advertisements/{id}` |
| Advertisement detail by slug | GET | `/api/advertisements/by-slug/{slug}` |
| Membership plans | GET | `/api/membership-plans` |
| Login | POST | `/api/account/login` |
| Registration | POST | `/api/account/register` |
| Current user | GET | `/api/account/authenticated-user` |
| Logout | POST | `/api/account/logout` |

## Customer-protected actions used from public UI

| Action | Method | API route |
|---|---|---|
| Add favorite | POST | `/api/customer/favorites/{advertisementId}` |
| Remove favorite | DELETE | `/api/customer/favorites/{advertisementId}` |
| Purchase membership | POST | `/api/customer/membership/purchase/{membershipPlanId}` |

## Advertisement search query contract

The frontend sends the actual API property names:

- `Keyword`
- `AdvertisementCategoryID`
- `ProvinceID`
- `CityID`
- `MinPrice`
- `MaxPrice`
- `IsImmediate`
- `Sort`
- `PageIndex`
- `PageSize`

Supported sort values from the current service implementation:

- `newest`
- `oldest`
- `price-asc`
- `price-desc`
- `most-viewed`

## Authentication response

`POST /api/account/login` returns:

- `token`
- `expiration`
- `userID`
- `firstName`
- `lastName`
- `mobileNumber`
- `avatarPath`
- `roles`

Remember Me rule follows the API README recommendation:

- checked → `localStorage`
- unchecked → `sessionStorage`

The current API does **not** expose a refresh-token endpoint, so the frontend does not invent one.

## Important business-rule mismatch

The uploaded API still contains `IsFeatured` / `FeaturedAdvertisements` for Admin/home aggregate purposes. The accepted Visitor UI rule is different: **Visitor Panel must not show ویژه at all.** Therefore this frontend deliberately ignores `featuredAdvertisements` and `isFeatured`. Only `isImmediate` is rendered as the **فوری** badge.

## Removed visitor functionality

The current API has no forgot-password endpoint, and the user explicitly removed that requirement. These pages are not part of this package:

- About
- Terms
- Privacy
- Forgot Password
- Contact
- Blog/News

A concise About/Rules/Privacy summary is shown in the shared footer instead.
