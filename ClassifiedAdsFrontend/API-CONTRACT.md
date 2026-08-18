# Complete Frontend ↔ Final ClassifiedAdsApi Contract

Source of truth: the uploaded final `ClassifiedAdsApi(1).zip` source code.

## Runtime

```text
API root:   http://localhost:5086/api
Media root: http://localhost:5086
```

Configured centrally in `assets/js/config.js`.

## Account

| Feature | Method | Route | Auth |
|---|---:|---|---|
| Register Customer | POST | `/api/account/register` | Public |
| Login | POST | `/api/account/login` | Public |
| Current user | GET | `/api/account/authenticated-user` | Bearer |
| Change password | POST | `/api/account/change-password` | Bearer |
| Logout | POST | `/api/account/logout` | Bearer |

Login result fields used: `token`, `expiration`, `userID`, `firstName`, `lastName`, `mobileNumber`, `avatarPath`, `roles`.

## Public Visitor API

| Feature | Method | Route |
|---|---:|---|
| Home aggregate | GET | `/api/site/home` |
| Public site settings | GET | `/api/site/settings` |
| Two-level category menu | GET | `/api/categories/menu` |
| Category details | GET | `/api/categories/{id}` |
| Category by slug | GET | `/api/categories/by-slug/{slug}` |
| Provinces | GET | `/api/provinces` |
| Province details | GET | `/api/provinces/{id}` |
| Cities by province | GET | `/api/cities/by-province/{provinceId}` |
| City details | GET | `/api/cities/{id}` |
| Public ads | GET | `/api/advertisements` |
| Public ad details | GET | `/api/advertisements/{id}` |
| Public ad by slug | GET | `/api/advertisements/by-slug/{slug}` |
| Active membership plans | GET | `/api/membership-plans` |
| Membership plan details | GET | `/api/membership-plans/{id}` |

Public advertisement search fields used:

```text
Keyword
AdvertisementCategoryID
ProvinceID
CityID
MinPrice
MaxPrice
IsImmediate
Sort
PageIndex
PageSize
```

Supported sort values verified in the final service:

```text
newest
oldest
price-asc
price-desc
most-viewed
```

## Customer API

All routes require role `Customer`.

| Feature | Method | Route |
|---|---:|---|
| My advertisements | GET | `/api/customer/advertisements` |
| My advertisement detail | GET | `/api/customer/advertisements/{id}` |
| Create Draft | POST | `/api/customer/advertisements` |
| Edit Draft/Rejected | PUT | `/api/customer/advertisements/{id}` |
| Delete owned ad | DELETE | `/api/customer/advertisements/{id}` |
| Submit for review | POST | `/api/customer/advertisements/{id}/submit` |
| Favorites | GET | `/api/customer/favorites` |
| Add favorite | POST | `/api/customer/favorites/{advertisementId}` |
| Remove favorite | DELETE | `/api/customer/favorites/{advertisementId}` |
| Current membership | GET | `/api/customer/membership` |
| Purchase membership | POST | `/api/customer/membership/purchase/{membershipPlanId}` |
| Customer payments | GET | `/api/customer/payments` |
| Upload avatar | POST multipart | `/api/file-manager/customer/avatar` |
| Upload ad image | POST multipart | `/api/file-manager/advertisements/{advertisementId}/images` |
| Delete ad image | DELETE | `/api/file-manager/advertisements/{advertisementId}/images/{imageId}` |
| Set main image | PATCH | `/api/file-manager/advertisements/{advertisementId}/images/{imageId}/main` |

Customer advertisement create/edit payload:

```text
title
description
price
phoneNumber
advertisementCategoryID
provinceID
cityID
isImmediate
```

Nullable SEO properties accepted by the API are deliberately not exposed to ordinary customers and are sent as null.

Customer status codes consumed:

```text
Draft
Pending
Published
Rejected
Expired
Disabled
```

## Admin API

All routes require role `Admin`.

### Dashboard / advertisements

| Feature | Method | Route |
|---|---:|---|
| Dashboard | GET | `/api/admin/dashboard` |
| Search ads | GET | `/api/admin/advertisements` |
| Review detail | GET | `/api/admin/advertisements/{id}` |
| Approve | PATCH | `/api/admin/advertisements/{id}/approve` |
| Reject | PATCH | `/api/admin/advertisements/{id}/reject` |
| Disable | PATCH | `/api/admin/advertisements/{id}/disable` |

The backend also exposes feature/unfeature endpoints, but this frontend intentionally does not expose the old `ویژه` concept.

### Customers

| Feature | Method | Route |
|---|---:|---|
| Customer list | GET | `/api/admin/customers` |
| Activate | PATCH | `/api/admin/customers/{id}/activate` |
| Deactivate | PATCH | `/api/admin/customers/{id}/deactivate` |

### Categories

```text
GET    /api/admin/categories
POST   /api/admin/categories
PUT    /api/admin/categories/{id}
DELETE /api/admin/categories/{id}
```

### Provinces

```text
GET    /api/admin/provinces
POST   /api/admin/provinces
PUT    /api/admin/provinces/{id}
DELETE /api/admin/provinces/{id}
```

### Cities

```text
GET    /api/admin/cities
POST   /api/admin/cities
PUT    /api/admin/cities/{id}
DELETE /api/admin/cities/{id}
```

### Membership plans

```text
GET    /api/admin/membership-plans
POST   /api/admin/membership-plans
PUT    /api/admin/membership-plans/{id}
DELETE /api/admin/membership-plans/{id}
```

### Payments

```text
GET /api/admin/payments
```

### Hero banners

```text
GET    /api/admin/hero-banners
POST   /api/admin/hero-banners
PUT    /api/admin/hero-banners/{id}
DELETE /api/admin/hero-banners/{id}
PATCH  /api/admin/hero-banners/{id}/activate
PATCH  /api/admin/hero-banners/{id}/deactivate
POST   /api/file-manager/hero-banners/{id}/image
```

### Site settings / files

```text
GET  /api/admin/site-setting
PUT  /api/admin/site-setting
POST /api/file-manager/site/logo
POST /api/file-manager/site/favicon
```

## Pagination

Paged API responses expose:

```text
items
pageModel.pageIndex
pageModel.pageSize
pageModel.recordCount
pageModel.pageCount
```

The frontend never downloads the complete database just to paginate.

## Media

The frontend receives backend-relative file paths and resolves them against:

```text
http://localhost:5086
```

It never uses backend physical disk paths.

## API gaps deliberately not faked

- No Messages/Notification API.
- No customer/admin name/mobile update API.
- No Admin avatar upload API.
- No Admin endpoint for editing advertisement content; Admin is a moderation workflow.
- Admin advertisement detail does not include customer identity even though Admin advertisement list does.
