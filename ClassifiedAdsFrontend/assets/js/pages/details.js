(function () {
  const host = document.getElementById("detailsHost");
  const similarHost = document.getElementById("similarAds");
  const breadcrumbHost = document.getElementById("detailsBreadcrumb");
  const params = new URLSearchParams(location.search);
  const id = params.get("id");
  const slug = params.get("slug");

  async function load() {
    host.innerHTML = window.UI.loadingMarkup();
    similarHost.innerHTML = `<div class="col-12">${window.UI.loadingMarkup()}</div>`;
    if (!id && !slug) { host.innerHTML = window.UI.emptyMarkup(); return; }
    try {
      const endpoint = slug ? window.AppConfig.endpoints.advertisementBySlug(slug) : window.AppConfig.endpoints.advertisementDetails(id);
      const response = await window.Api.get(endpoint);
      const ad = response.data;
      render(ad);
      window.UI.applySeo(ad);
      await loadSimilar(ad);
      await window.Favorites.handlePending();
    } catch (error) {
      const e = window.Api.normalizeError(error);
      host.innerHTML = e.status === 404 ? window.UI.emptyMarkup() : window.UI.errorMarkup(e.message);
      host.querySelector("[data-retry]")?.addEventListener("click", load, { once: true });
      similarHost.innerHTML = "";
    }
  }

  function render(ad) {
    const images = Array.isArray(ad.images) ? ad.images : [];
    const main = images.find(x => x.isMainImage) || images[0];
    const mainUrl = window.UI.mediaUrl(main && main.imagePath, window.AppConfig.defaultAdvertisementImage);
    window.Breadcrumb.render(breadcrumbHost, Array.isArray(ad.breadcrumb) && ad.breadcrumb.length ? ad.breadcrumb : [{ title: "خانه", url: "index.html" }, { title: "آگهی‌ها", url: "advertisements.html" }, { title: ad.title, url: null }]);
    host.innerHTML = `<div class="details-layout">
      <section class="details-gallery"><div class="gallery-main"><img id="galleryMain" src="${window.UI.escapeHtml(mainUrl)}" alt="${window.UI.escapeHtml(ad.title)}" onerror="this.src='${window.AppConfig.defaultAdvertisementImage}'"></div><div class="gallery-thumbs">${images.map((image, index) => `<button class="gallery-thumb ${index === 0 ? "is-active" : ""}" data-src="${window.UI.escapeHtml(window.UI.mediaUrl(image.imagePath, window.AppConfig.defaultAdvertisementImage))}" type="button"><img src="${window.UI.escapeHtml(window.UI.mediaUrl(image.thumbnailPath || image.imagePath, window.AppConfig.defaultAdvertisementImage))}" alt="${window.UI.escapeHtml(image.altText || ad.title)}"></button>`).join("")}</div><div class="content-card description-card"><h2>توضیحات آگهی</h2><p>${window.UI.escapeHtml(ad.description || "توضیحی ثبت نشده است.").replace(/\n/g, "<br>")}</p></div></section>
      <aside class="details-sidebar"><div class="content-card details-summary">${ad.isImmediate ? '<span class="urgent-badge urgent-badge--inline">فوری</span>' : ''}<h1>${window.UI.escapeHtml(ad.title)}</h1><div class="details-price">${window.UI.formatPrice(ad.price)}</div><dl><div><dt>دسته‌بندی</dt><dd>${window.UI.escapeHtml(ad.categoryName)}</dd></div><div><dt>موقعیت</dt><dd>${window.UI.escapeHtml(`${ad.provinceName}، ${ad.cityName}`)}</dd></div><div><dt>تاریخ انتشار</dt><dd>${window.UI.escapeHtml(ad.publishDatePersian || ad.createDatePersian || "")}</dd></div><div><dt>بازدید</dt><dd>${Number(ad.viewCount || 0).toLocaleString("fa-IR")}</dd></div></dl><div class="phone-card"><span id="phoneValue">${maskPhone(ad.phoneNumber)}</span><button class="btn btn-primary btn-sm" id="revealPhone" type="button">نمایش شماره تماس</button></div><button class="favorite-detail ${ad.isFavorite ? "is-active" : ""}" data-favorite-id="${ad.advertisementID}" type="button">${window.UI.icon("heart", "currentColor")}<span>${ad.isFavorite ? "ذخیره شده" : "ذخیره آگهی"}</span></button></div></aside>
    </div>`;
    document.getElementById("revealPhone").addEventListener("click", event => { document.getElementById("phoneValue").textContent = ad.phoneNumber || "شماره تماس ثبت نشده"; event.currentTarget.hidden = true; });
    host.querySelector(".gallery-thumbs")?.addEventListener("click", event => {
      const btn = event.target.closest(".gallery-thumb"); if (!btn) return;
      document.getElementById("galleryMain").src = btn.dataset.src;
      host.querySelectorAll(".gallery-thumb").forEach(x => x.classList.remove("is-active")); btn.classList.add("is-active");
    });
    window.Favorites.bind(host);
  }

  function maskPhone(phone) {
    if (!phone) return "شماره تماس ثبت نشده";
    return `${phone.slice(0, 4)} *** **${phone.slice(-2)}`;
  }

  async function loadSimilar(ad) {
    try {
      const response = await window.Api.get(window.AppConfig.endpoints.advertisements, { params: { AdvertisementCategoryID: ad.advertisementCategoryID, PageIndex: 1, PageSize: 4, Sort: "newest" } });
      const items = (response.data.items || []).filter(x => x.advertisementID !== ad.advertisementID).slice(0, 3);
      similarHost.innerHTML = items.length ? items.map(window.UI.adCard).join("") : `<div class="col-12">${window.UI.emptyMarkup()}</div>`;
      window.Favorites.bind(similarHost);
    } catch { similarHost.innerHTML = `<div class="col-12">${window.UI.emptyMarkup()}</div>`; }
  }

  load();
})();
