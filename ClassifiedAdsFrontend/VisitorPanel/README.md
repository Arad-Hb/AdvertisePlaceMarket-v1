# تحلیل داده آگهی — Visitor Panel

Responsive Persian RTL Visitor frontend wired to the uploaded final ASP.NET Core API.

## Included pages

- `index.html` — Home
- `advertisements.html` — Search / filter / sort / pagination
- `advertisement-details.html` — Detail / gallery / phone reveal / favorite / similar ads
- `categories.html` — API-driven two-level categories
- `pricing.html` — API-driven membership plans + authenticated purchase action
- `login.html` — Login and Register in one responsive page
- `404.html` — Shared-layout not-found page

All pages except `login.html` use the same dynamic header/footer.

## Start the final API

From the uploaded API solution root:

```bash
dotnet run --project Api --launch-profile http
```

Expected API URL:

```text
http://localhost:5086
```

Swagger:

```text
http://localhost:5086/swagger
```

## Serve the frontend

Do not rely on `file://` for final testing. From this frontend folder run any simple local HTTP server, for example Python:

```bash
python -m http.server 5500
```

Then open:

```text
http://localhost:5500/index.html
```

CORS in the uploaded API uses the `Frontend` policy with `AllowAnyOrigin/AllowAnyHeader/AllowAnyMethod` for this educational setup.

## API host configuration

Edit only:

```text
assets/js/config.js
```

Defaults:

```text
apiBaseUrl   = http://localhost:5086/api
mediaBaseUrl = http://localhost:5086
```

## Seeded demo accounts from the API

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

## Visitor UI decisions implemented

- Axios only; no Fetch API.
- Bootstrap 5 RTL + Vanilla JavaScript.
- Fixed shared header and minimal footer.
- Mobile header: burger right, centered search, icon-only login/user left, no logo.
- Footer has the same blue family as the header and no background image.
- About/Rules/Privacy are summarized in one footer column.
- No Contact / About / Terms / Privacy / Forgot Password pages.
- Dynamic two-level Mega Menu from `/api/categories/menu`.
- Smooth custom dropdowns with no search box.
- Province → City is API dependent; City is disabled until Province is selected.
- Inputs/dropdowns use shadow instead of visible borders.
- Smooth toast transitions.
- API loading state text: `اطلاعات در حال بارگذاری می باشند..`
- Empty state text: `موردی یافت نشد` with skeleton treatment.
- Only `فوری` is rendered in Visitor UI.
- `IsFeatured` from the API is intentionally ignored in Visitor UI.
- Advertisement images use backend media paths; frontend images are static UI assets only.
- Login form and registration form match the real final API models.
- Login page has no site header/footer and fits desktop viewport without unnecessary page scrolling; mobile naturally stacks the visual below the form.

## Future Customer/Admin panels

Shared files are intentionally reusable:

- `assets/js/config.js`
- `assets/js/api.js`
- `assets/js/auth.js`
- `assets/js/ui.js`
- `assets/js/select-menu.js`
- `assets/js/categories.js`
- `assets/js/location.js`

Customer/Admin pages are not included yet.

## Validation performed in this package

- All JavaScript files passed `node --check`.
- All local HTML `src`/`href` asset references were checked and exist.
- Visitor endpoint routes were compared with the API-generated `ApiEndpoints.json` metadata.
- Advertisement search parameter names were compared with the generated endpoint metadata.
- The source was scanned to ensure Visitor code does not contain `fetch(`, `IsFeatured`, `ویژه`, `توافقی`, removed static-page links, or Forgot Password links.

An actual live database/API browser test still requires running the .NET 10 API with SQL Server on the target Windows environment. The current execution container does not have the .NET SDK or SQL Server, so it cannot boot that API here.
