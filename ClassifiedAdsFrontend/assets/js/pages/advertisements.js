(function () {
  const grid = document.getElementById("adsGrid");
  const paginationHost = document.getElementById("pagination");
  const countHost = document.getElementById("resultsCount");
  const breadcrumbHost = document.getElementById("listBreadcrumb");
  const forms = [document.getElementById("filterForm"), document.getElementById("mobileFilterForm")].filter(Boolean);
  const sortSelects = [document.getElementById("sortSelectDesktop"), document.getElementById("sortSelectMobile")].filter(Boolean);
  const q = new URLSearchParams(location.search);

  function currentParams(page) {
    const params = {
      Keyword: q.get("Keyword") || "",
      AdvertisementCategoryID: q.get("AdvertisementCategoryID") || "",
      ProvinceID: q.get("ProvinceID") || "",
      CityID: q.get("CityID") || "",
      MinPrice: q.get("MinPrice") || "",
      MaxPrice: q.get("MaxPrice") || "",
      IsImmediate: q.get("IsImmediate") || "",
      Sort: q.get("Sort") || "newest",
      PageIndex: page || Number(q.get("PageIndex") || 1),
      PageSize: window.AppConfig.pageSize
    };
    Object.keys(params).forEach(key => params[key] === "" && delete params[key]);
    return params;
  }

  function updateUrl(values) {
    const next = new URLSearchParams();
    Object.entries(values).forEach(([key, value]) => {
      if (value !== "" && value !== null && value !== undefined && value !== false) next.set(key, value);
    });
    history.pushState({}, "", `${location.pathname}?${next.toString()}`);
    location.reload();
  }

  async function prepareForms() {
    const [categories, provinces] = await Promise.all([window.Categories.getMenu(), window.LocationService.getProvinces()]);
    for (const form of forms) {
      const cat = form.querySelector('[name="AdvertisementCategoryID"]');
      const province = form.querySelector('[name="ProvinceID"]');
      const city = form.querySelector('[name="CityID"]');
      window.Categories.fillSelect(cat, categories, "همه دسته‌بندی‌ها");
      window.LocationService.fillProvinceSelect(province, provinces);
      cat.value = q.get("AdvertisementCategoryID") || "";
      province.value = q.get("ProvinceID") || "";
      window.SelectMenu.enhance(cat)?.sync();
      window.SelectMenu.enhance(province)?.sync();
      await window.LocationService.bindDependent(province, city, q.get("CityID"));
      form.querySelector('[name="Keyword"]').value = q.get("Keyword") || "";
      form.querySelector('[name="MinPrice"]').value = q.get("MinPrice") || "";
      form.querySelector('[name="MaxPrice"]').value = q.get("MaxPrice") || "";
      form.querySelector('[name="IsImmediate"]').checked = q.get("IsImmediate") === "true";
      form.addEventListener("submit", event => {
        event.preventDefault();
        const fd = new FormData(form);
        updateUrl({
          Keyword: fd.get("Keyword"), AdvertisementCategoryID: fd.get("AdvertisementCategoryID"),
          ProvinceID: fd.get("ProvinceID"), CityID: fd.get("CityID"), MinPrice: fd.get("MinPrice"), MaxPrice: fd.get("MaxPrice"),
          IsImmediate: form.querySelector('[name="IsImmediate"]').checked ? "true" : "", Sort: fd.get("Sort") || q.get("Sort") || "newest", PageIndex: 1
        });
      });
      form.querySelector("[data-clear-filters]")?.addEventListener("click", () => location.href = "advertisements.html");
    }
    sortSelects.forEach(select => {
      select.value = q.get("Sort") || "newest";
      window.SelectMenu.enhance(select)?.sync();
      select.addEventListener("change", () => updateUrl({ ...currentParams(1), Sort: select.value, PageIndex: 1, PageSize: undefined }));
    });
  }

  async function loadAds(page) {
    grid.innerHTML = `<div class="col-12">${window.UI.loadingMarkup()}</div>`;
    countHost.textContent = "";
    try {
      const response = await window.Api.get(window.AppConfig.endpoints.advertisements, { params: currentParams(page) });
      const data = response.data || {};
      const items = Array.isArray(data.items) ? data.items : [];
      const pageModel = data.pageModel || { pageIndex: 1, pageCount: 0, recordCount: 0 };
      countHost.textContent = `${Number(pageModel.recordCount || 0).toLocaleString("fa-IR")} آگهی`;
      grid.innerHTML = items.length ? items.map(window.UI.adCard).join("") : `<div class="col-12">${window.UI.emptyMarkup()}</div>`;
      window.Favorites.bind(grid);
      window.Pagination.render(paginationHost, pageModel, selectedPage => updateUrl({ ...currentParams(selectedPage), PageIndex: selectedPage, PageSize: undefined }));
      const crumbs = [{ title: "خانه", url: "index.html" }, { title: "آگهی‌ها", url: null }];
      if (Array.isArray(data.breadcrumb) && data.breadcrumb.length) {
        window.Breadcrumb.render(breadcrumbHost, [{ title: "خانه", url: "index.html" }, { title: "آگهی‌ها", url: "advertisements.html" }, ...data.breadcrumb]);
      } else window.Breadcrumb.render(breadcrumbHost, crumbs);
      await window.Favorites.handlePending();
    } catch (error) {
      grid.innerHTML = `<div class="col-12">${window.UI.errorMarkup(window.Api.normalizeError(error).message)}</div>`;
      grid.querySelector("[data-retry]")?.addEventListener("click", () => loadAds(page), { once: true });
    }
  }

  prepareForms().then(() => window.SelectMenu.enhanceAll()).catch(error => window.UI.showToast(window.Api.normalizeError(error).message, "error"));
  loadAds(Number(q.get("PageIndex") || 1));
})();
