# Frontend Validation Report

Validation target:

- Frontend: this complete Visitor + Customer + Admin package
- API source: uploaded `ClassifiedAdsApi(1).zip`

## Completed checks

### 1. API source inspection

The following were inspected directly from the final API source:

- `Program.cs`
- launch settings (`http://localhost:5086`)
- AccountController
- CustomerController
- AdminController
- FileManagerController
- AdvertisementCategoryController
- AdvertisementController
- ProvinceController
- CityController
- MembershipController
- SiteController
- Account DTOs
- Advertisement public/customer/admin DTOs
- Membership DTOs
- Payment DTOs
- Category/Province/City DTOs
- SiteSetting/HeroBanner DTOs
- advertisement business rules in `AdvertisementService`
- membership purchase semantics in `MembershipService`

### 2. JavaScript syntax

All JavaScript files in `assets/js` were checked with Node's parser (`node --check`).

Result: **syntax valid**.

### 3. Local asset references

All local HTML `script[src]`, stylesheet `link[href]` and `img[src]` references were scanned.

Result: **no missing local asset reference**.

### 4. Removed/forbidden implementation checks

Runtime HTML/CSS/JS was scanned for:

```text
fetch(
IsFeatured
isFeatured
ویژه
توافقی
forgot-password
about.html
terms.html
privacy.html
```

Result: **none remain as runtime frontend features**.

Axios is the only HTTP client used.

### 5. Role and navigation integration

Verified in the shared auth/layout code:

- Public visitor → login → returnUrl
- Customer default login target → `customer/index.html`
- Admin default login target → `admin/index.html`
- Customer `ثبت آگهی` → customer creation workflow
- Admin public header CTA → Admin management instead of incorrectly showing Customer ad creation
- Public pricing → Customer membership panel
- Panel `بازگشت به فروشگاه` → Visitor home
- Panel logout → Public home
- Customer published-ad link → Visitor detail page
- Customer favorites reuse public advertisement identity

### 6. API model alignment

Verified:

- Login payload matches `LoginModel`.
- Register payload matches `RegisterModel`.
- Change-password payload matches `ChangePasswordModel`.
- Customer add/edit ad fields match `AdvertisementAddModel` / `AdvertisementEditModel`.
- Customer status logic matches `Draft/Pending/Published/Rejected/Expired/Disabled`.
- Edit UI is restricted to Draft/Rejected, matching the service.
- Admin moderation buttons follow API business rules.
- Membership UI uses `CurrentMembershipModel` and real remaining quota.
- Payment tables use `PaymentListItem`.
- Category/Province/City/Admin forms use their real add/edit DTOs.
- Hero banner and site-setting forms use their real final DTOs.

### 7. Responsive/static design review

Panel CSS includes dedicated breakpoints for desktop/tablet/mobile and supports:

- right-side full sidebar
- icon-only collapsed sidebar
- avatar preserved in collapsed state
- mobile slide-in sidebar + overlay
- header that does not overlay the sidebar
- responsive cards/grids/tables/forms
- no intentional horizontal page scrolling; wide data tables scroll inside their own wrapper only

## Live .NET database test limitation

The execution environment used to build this package does not provide the .NET runtime/SDK or SQL Server. Therefore the uploaded API cannot be booted here for a genuine database-backed browser end-to-end run.

The frontend was instead wired by direct inspection of the **actual final controller routes, DTOs and service rules**, not by older prompts or guessed endpoints.

For the final runtime test on the Windows development machine:

1. Start the final API on `http://localhost:5086`.
2. Open Swagger and confirm database seeding completed.
3. Serve this frontend over a simple local HTTP server.
4. Test the seeded Admin and Customer accounts listed in `README.md`.

## Known API limitations surfaced to the UI

- No Messages/Notifications API → no fake messaging pages/actions.
- No identity profile-edit endpoint → names/mobile stay read-only.
- Customer avatar upload only → Admin has no fake avatar upload.
- Admin detail endpoint lacks customer identity → list page has customer info, review detail uses only available detail fields.
