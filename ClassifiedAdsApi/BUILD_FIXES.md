# Build fixes applied

This revision fixes the compiler/runtime issues reported after the first package.

## Fixed

1. Namespace/type collisions
   - `AdvertisementCategory` in `IAdvertisementRepository` and `AdvertisementRepository`
   - `Province` in `IAdvertisementRepository` and `AdvertisementRepository`
   - `City` in `IAdvertisementRepository` and `AdvertisementRepository`
   - proactively fixed the same collision pattern for `SiteSetting`
   - affected domain entity types are explicitly qualified where a sibling namespace can shadow the type name.

2. Duplicate ViewModel member warning
   - removed the second `Icon` declaration from `AdvertisementCategoryDetailsModel`.
   - the inherited `AdvertisementCategoryListItem.Icon` property remains available.

3. SixLabors license error
   - removed `SixLabors.ImageSharp` completely.
   - no SixLabors package or namespace remains in the solution.
   - thumbnails use `System.Drawing.Common` for JPG/JPEG/PNG on Windows.
   - WebP and non-Windows thumbnail creation use a separate copied file because GDI+ does not provide built-in WebP support.

4. Cascading `DataAccess.dll could not be found`
   - this was caused by DataAccess build failures above; it is not a separate missing DLL that should be manually copied.

## Before testing this fixed package

Use a fresh extracted folder. If you overwrite the old folder, delete all `bin` and `obj` directories first, then run:

```powershell
dotnet clean
dotnet restore
dotnet build ClassifiedAds.sln
```

Then run:

```powershell
dotnet run --project Api --launch-profile http
```
