# تحلیل داده آگهی — Complete Frontend

This package contains the complete Persian RTL frontend for the uploaded final `ClassifiedAdsApi(1).zip` API.

The frontend is intentionally simple and educational:

- HTML5
- CSS3
- Bootstrap 5 RTL
- Vanilla JavaScript / ECMAScript
- Axios only
- No Fetch API
- No jQuery
- No React/Vue/Angular/TypeScript

## Project areas

### Visitor Panel

- `index.html` — Home
- `advertisements.html` — Public search/filter/sort/pagination
- `advertisement-details.html` — Public advertisement details/gallery/favorite
- `categories.html` — Two-level API categories
- `pricing.html` — Public membership plans
- `login.html` — Login + Customer registration in one page
- `404.html`

The Visitor Panel keeps the previously approved visual direction, common header/footer, responsive search/navigation, API loading/empty/toast behavior and dependent Province → City selection.

### Customer Panel

- `customer/index.html` — Dashboard
- `customer/advertisements.html` — My advertisements
- `customer/create-advertisement.html` — Create advertisement
- `customer/edit-advertisement.html` — Edit Draft/Rejected advertisement + image management
- `customer/favorites.html` — Favorites
- `customer/membership.html` — Current membership + purchase plans
- `customer/payments.html` — Payment history
- `customer/profile.html` — Profile + customer avatar upload
- `customer/settings.html` — Change password

### Admin Panel

- `admin/index.html` — Dashboard
- `admin/advertisements.html` — Advertisement moderation list
- `admin/advertisement-review.html` — Approve / Reject / Disable advertisement
- `admin/customers.html` — Activate / Deactivate customers
- `admin/categories.html` — Category CRUD
- `admin/provinces.html` — Province CRUD
- `admin/cities.html` — City CRUD
- `admin/membership-plans.html` — Membership plan CRUD
- `admin/payments.html` — Payment list/filter/pagination
- `admin/hero-banners.html` — Hero banner CRUD + image upload + activate/deactivate
- `admin/site-settings.html` — General site/SEO/social settings + logo/favicon upload
- `admin/account.html` — Admin account summary + change password

## Customer/Admin visual system

The panels are rebuilt from scratch in Bootstrap 5/Vanilla JS using the approved Pluto-inspired design language:

- RTL sidebar fixed on the **right**
- Sidebar extends to the top of the viewport
- First sidebar row uses `#214162` and shows user avatar/name/role
- Sidebar collapses from full width to icon-only mode
- The user avatar remains visible when collapsed
- Dark navy header/sidebar palette based on the uploaded Pluto screenshots
- Orange `#ff5a23` collapse/menu accent
- TahlilDadeh logo + `تحلیل داده آگهی` replaces Pluto branding
- `بازگشت به فروشگاه` remains in the panel header and sidebar
- Colorful functional sidebar icons
- Responsive mobile sidebar overlay/offcanvas behavior
- Compact white cards, small shadows, minimal form controls
- No decorative fake dashboard modules that are unsupported by the API

## API configuration

All routes are centralized in:

```text
assets/js/config.js
```

Default API host from the uploaded API launch profile:

```text
http://localhost:5086
```

Frontend configuration:

```text
apiBaseUrl   = http://localhost:5086/api
mediaBaseUrl = http://localhost:5086
```

If your API host changes, edit those values in one file only.

## Start the API

From the uploaded API solution root:

```bash
dotnet run --project Api --launch-profile http
```

Swagger should be available at:

```text
http://localhost:5086/swagger
```

The API uses a development CORS policy that allows the separate frontend origin.

## Serve the frontend

A local HTTP server is recommended instead of opening pages through `file://`:

```bash
python -m http.server 5500
```

Then open:

```text
http://localhost:5500/index.html
```

## Seed API accounts

Admin:

```text
09120000001
Admin@123456
```

Customer:

```text
09120000002
Customer@123456
```

## Visitor → Customer/Admin integration

The three frontend areas use the same:

- JWT/session storage
- current-user model
- API client
- media URL helper
- toast/error/loading patterns
- category/location data helpers

Behavior:

- Visitor `ثبت آگهی` → Login when unauthenticated → Customer create-ad page after login.
- Logged-in Customer → Customer panel and Customer create-ad page.
- Logged-in Admin → Admin dashboard.
- Visitor pricing plan selection → Customer membership page; it does not invent a public checkout flow.
- Customer favorites open the same Visitor advertisement-details page.
- Published customer advertisements can be viewed in the Visitor Panel.
- Logout from either panel returns to the public site.

## Advertisement create/edit workflow

The frontend follows the real API workflow instead of faking a single giant request:

```text
Create Draft
    ↓
receive AdvertisementID
    ↓
upload images one by one
    ↓
choose main image
    ↓
submit advertisement for review
```

Customer editing is allowed in the UI only for `Draft` and `Rejected`, matching the backend service rule.

## Important final API limitations respected by the frontend

These are API capabilities, not frontend bugs:

1. **No Messages/Notifications API**
   - No fake Messages page was created.
   - No nonfunctional notification counter is shown.

2. **No endpoint to edit customer/admin FirstName, LastName or MobileNumber**
   - Identity fields are displayed read-only.
   - The frontend does not fake profile saving.

3. **Customer avatar upload exists; Admin avatar upload does not**
   - Customer profile supports real avatar upload.
   - Admin account shows the current avatar but does not show a fake upload action.

4. **Admin advertisement-detail DTO does not return customer identity**
   - Customer name/mobile are shown on the Admin advertisement list where the API returns them.
   - The review-details endpoint itself displays only fields it actually provides.

5. **The API still contains `IsFeatured`**
   - Per the accepted frontend rule, Visitor/Customer UI intentionally ignores `IsFeatured` / `ویژه`.
   - Only `IsImmediate` is presented as `فوری`.
   - The API was not changed.

## JavaScript structure

Shared:

```text
assets/js/config.js
assets/js/api.js
assets/js/auth.js
assets/js/ui.js
assets/js/categories.js
assets/js/location.js
```

Visitor:

```text
assets/js/layout.js
assets/js/select-menu.js
assets/js/pages/*
```

Panel shared:

```text
assets/js/panel-ui.js
assets/js/panel-layout.js
```

Customer:

```text
assets/js/customer/*
```

Admin:

```text
assets/js/admin/*
```

The code deliberately avoids repositories/state frameworks/event buses/frontend dependency injection so beginner developers can follow each request → response → render flow.

## Validation

See `VALIDATION.md` for static validation details and the exact live-run limitation of this environment.
