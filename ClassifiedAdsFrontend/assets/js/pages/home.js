(function () {
  const categoryHost = document.getElementById("homeCategories");
  const latestHost = document.getElementById("latestAds");
  const categorySelect = document.getElementById("homeCategory");
  const provinceSelect = document.getElementById("homeProvince");
  const citySelect = document.getElementById("homeCity");
  const form = document.getElementById("homeSearchForm");

  async function load() {
    categoryHost.innerHTML = window.UI.loadingMarkup();
    latestHost.innerHTML = `<div class="col-12">${window.UI.loadingMarkup()}</div>`;
    try {
      const [home, provinces] = await Promise.all([window.SiteData.getHome(), window.LocationService.getProvinces()]);
      const categories = Array.isArray(home.categories) ? home.categories : [];
      renderHero(home.heroBanners || []);
      renderCategories(categories);
      renderLatest(home.latestAdvertisements || []);
      window.Categories.fillSelect(categorySelect, categories, "انتخاب دسته‌بندی");
      window.LocationService.fillProvinceSelect(provinceSelect, provinces);
      await window.LocationService.bindDependent(provinceSelect, citySelect);
      window.SelectMenu.enhanceAll();
      if (home.siteSetting) applySiteSetting(home.siteSetting);
    } catch (error) {
      const message = window.Api.normalizeError(error).message;
      categoryHost.innerHTML = window.UI.errorMarkup(message);
      latestHost.innerHTML = `<div class="col-12">${window.UI.errorMarkup(message)}</div>`;
      document.querySelectorAll("[data-retry]").forEach(btn => btn.addEventListener("click", load, { once: true }));
    }
  }

  function renderHero(banners) {
    const first = banners.find(x => x.isActive !== false) || banners[0];
    if (!first) return;
    if (first.title) document.getElementById("heroTitle").textContent = first.title;
    if (first.subtitle) document.getElementById("heroSubtitle").textContent = first.subtitle;
  }

  function applySiteSetting(setting) {
    if (setting.defaultSeoTitle) document.title = setting.defaultSeoTitle;
    const meta = document.querySelector('meta[name="description"]');
    if (meta && setting.defaultSeoDescription) meta.content = setting.defaultSeoDescription;
  }

  function renderCategories(categories) {
    if (!categories.length) { categoryHost.innerHTML = window.UI.emptyMarkup(); return; }
    categoryHost.innerHTML = categories.slice(0, 8).map((item, index) => {
      const visual = window.UI.categoryVisual(item.categoryName, index);
      return `<a class="home-category" href="${window.Categories.categoryUrl(item.advertisementCategoryID)}"><span class="home-category__icon" style="--cat-color:${visual.color}">${window.UI.icon(visual.iconName, "#28a9f5")}</span><strong>${window.UI.escapeHtml(item.categoryName)}</strong><small>${Number(item.advertisementCount || 0).toLocaleString("fa-IR")} آگهی</small></a>`;
    }).join("");
  }

  function renderLatest(items) {
    if (!items.length) { latestHost.innerHTML = `<div class="col-12">${window.UI.emptyMarkup()}</div>`; return; }
    latestHost.innerHTML = items.slice(0, 6).map(window.UI.adCard).join("");
    window.Favorites.bind(latestHost);
  }

  form.addEventListener("submit", function (event) {
    event.preventDefault();
    const data = new FormData(form);
    const q = new URLSearchParams();
    const map = {
      keyword: "Keyword",
      advertisementCategoryID: "AdvertisementCategoryID",
      provinceID: "ProvinceID",
      cityID: "CityID"
    };
    for (const [key, value] of data.entries()) {
      if (String(value).trim()) q.set(map[key] || key, String(value).trim());
    }
    location.href = `advertisements.html${q.toString() ? `?${q}` : ""}`;
  });

  load();
})();
