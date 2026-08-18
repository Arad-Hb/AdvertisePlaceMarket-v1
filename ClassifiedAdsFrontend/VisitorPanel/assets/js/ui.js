window.UI = (function () {
  function escapeHtml(value) {
    return String(value ?? "").replace(/[&<>'"]/g, c => ({"&":"&amp;","<":"&lt;",">":"&gt;","'":"&#39;",'"':"&quot;"})[c]);
  }
  function mediaUrl(path, fallback) {
    if (!path) return fallback || window.AppConfig.defaultAdvertisementImage;
    if (/^https?:\/\//i.test(path)) return path;
    return `${window.AppConfig.mediaBaseUrl.replace(/\/$/, "")}/${String(path).replace(/^\//, "")}`;
  }
  function formatPrice(value) {
    if (value === null || value === undefined || value === "") return "قیمت درج نشده";
    return `${Number(value).toLocaleString("fa-IR")} تومان`;
  }
  function icon(name, color) {
    const c = color || "currentColor";
    const icons = {
      search:`<svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="6.5" fill="none" stroke="${c}" stroke-width="1.8"/><path d="m16 16 4 4" stroke="${c}" stroke-width="1.8" stroke-linecap="round"/></svg>`,
      user:`<svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="12" cy="8" r="3.5" fill="none" stroke="${c}" stroke-width="1.8"/><path d="M5 20c.7-4 3-6 7-6s6.3 2 7 6" fill="none" stroke="${c}" stroke-width="1.8" stroke-linecap="round"/></svg>`,
      lock:`<svg viewBox="0 0 24 24" aria-hidden="true"><rect x="5" y="10" width="14" height="10" rx="2.5" fill="none" stroke="${c}" stroke-width="1.8"/><path d="M8 10V7a4 4 0 0 1 8 0v3" fill="none" stroke="${c}" stroke-width="1.8"/></svg>`,
      mail:`<svg viewBox="0 0 24 24" aria-hidden="true"><rect x="3.5" y="5" width="17" height="14" rx="2.5" fill="none" stroke="${c}" stroke-width="1.8"/><path d="m5 7 7 6 7-6" fill="none" stroke="${c}" stroke-width="1.8"/></svg>`,
      phone:`<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M7 3h3l1 5-2 1c1 3 3 5 6 6l1-2 5 1v3c0 2-2 4-4 4C9 20 4 15 3 7c0-2 2-4 4-4Z" fill="none" stroke="${c}" stroke-width="1.7" stroke-linejoin="round"/></svg>`,
      pin:`<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M20 10c0 5-8 11-8 11S4 15 4 10a8 8 0 1 1 16 0Z" fill="none" stroke="${c}" stroke-width="1.7"/><circle cx="12" cy="10" r="2.4" fill="none" stroke="${c}" stroke-width="1.7"/></svg>`,
      grid:`<svg viewBox="0 0 24 24" aria-hidden="true"><rect x="4" y="4" width="6" height="6" rx="1" fill="none" stroke="${c}" stroke-width="1.7"/><rect x="14" y="4" width="6" height="6" rx="1" fill="none" stroke="${c}" stroke-width="1.7"/><rect x="4" y="14" width="6" height="6" rx="1" fill="none" stroke="${c}" stroke-width="1.7"/><rect x="14" y="14" width="6" height="6" rx="1" fill="none" stroke="${c}" stroke-width="1.7"/></svg>`,
      home:`<svg viewBox="0 0 24 24" aria-hidden="true"><path d="m4 11 8-7 8 7v9h-6v-6h-4v6H4v-9Z" fill="none" stroke="${c}" stroke-width="1.7" stroke-linejoin="round"/></svg>`,
      briefcase:`<svg viewBox="0 0 24 24" aria-hidden="true"><rect x="3" y="7" width="18" height="13" rx="2" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M8 7V4h8v3M3 12h18" fill="none" stroke="${c}" stroke-width="1.7"/></svg>`,
      gear:`<svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="12" cy="12" r="3" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M12 2v3M12 19v3M2 12h3M19 12h3M5 5l2 2M17 17l2 2M19 5l-2 2M7 17l-2 2" fill="none" stroke="${c}" stroke-width="1.7" stroke-linecap="round"/></svg>`,
      car:`<svg viewBox="0 0 24 24" aria-hidden="true"><path d="m5 11 2-5h10l2 5 2 2v5h-2v2h-3v-2H8v2H5v-2H3v-5l2-2Z" fill="none" stroke="${c}" stroke-width="1.7"/><circle cx="7" cy="14" r="1" fill="${c}"/><circle cx="17" cy="14" r="1" fill="${c}"/></svg>`,
      device:`<svg viewBox="0 0 24 24" aria-hidden="true"><rect x="4" y="5" width="11" height="9" rx="1.5" fill="none" stroke="${c}" stroke-width="1.7"/><rect x="16" y="7" width="5" height="11" rx="1.2" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M7 18h5" stroke="${c}" stroke-width="1.7" stroke-linecap="round"/></svg>`,
      heart:`<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M20.8 4.7a5.3 5.3 0 0 0-7.5 0L12 6l-1.3-1.3a5.3 5.3 0 0 0-7.5 7.5L12 21l8.8-8.8a5.3 5.3 0 0 0 0-7.5Z" fill="none" stroke="${c}" stroke-width="1.7"/></svg>`,
      arrowLeft:`<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M19 12H5m6-6-6 6 6 6" fill="none" stroke="${c}" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/></svg>`,
      menu:`<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 7h16M4 12h16M4 17h16" fill="none" stroke="${c}" stroke-width="1.9" stroke-linecap="round"/></svg>`,
      check:`<svg viewBox="0 0 24 24" aria-hidden="true"><path d="m5 12 4 4L19 6" fill="none" stroke="${c}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>`
    };
    return icons[name] || icons.grid;
  }
  function categoryVisual(name, index) {
    const n = String(name || "");
    const palette = ["#ef5350", "#f3bd0a", "#9bc53d", "#25c46a", "#168bd6", "#15b6cd", "#7e57c2", "#8d4c39", "#ab47bc", "#ff7043"];
    let iconName = "grid";
    if (/املاک|خانه|ساختمان/.test(n)) iconName = "home";
    else if (/استخدام|بازرگانی|کسب/.test(n)) iconName = "briefcase";
    else if (/حمل|نقل|خودرو|وسیله/.test(n)) iconName = "car";
    else if (/دیجیتال|الکترونیک/.test(n)) iconName = "device";
    else if (/خدمت|صنعت|چاپ/.test(n)) iconName = "gear";
    else if (/آموزش/.test(n)) iconName = "user";
    return { color: palette[index % palette.length], iconName };
  }
  function loadingMarkup() {
    return `<div class="api-loading" role="status"><div class="api-loader"><span></span><span></span><span></span></div><div>اطلاعات در حال بارگذاری می باشند..</div></div>`;
  }
  function emptyMarkup() {
    return `<div class="empty-skeleton"><div class="empty-skeleton__shape"><i></i><i></i><i></i></div><strong>موردی یافت نشد</strong></div>`;
  }
  function errorMarkup(message) {
    return `<div class="api-error"><div class="api-error__icon">!</div><strong>دریافت اطلاعات انجام نشد</strong><span>${escapeHtml(message || "لطفاً دوباره تلاش کنید.")}</span><button class="btn btn-primary btn-sm" data-retry>تلاش مجدد</button></div>`;
  }
  function showToast(message, type) {
    let host = document.getElementById("appToastHost");
    if (!host) {
      host = document.createElement("div");
      host.id = "appToastHost";
      host.className = "app-toast-host";
      document.body.appendChild(host);
    }
    const toast = document.createElement("div");
    toast.className = `app-toast app-toast--${type || "info"}`;
    toast.innerHTML = `<span class="app-toast__mark">${type === "success" ? icon("check", "#fff") : "!"}</span><span>${escapeHtml(message)}</span><button type="button" aria-label="بستن">×</button>`;
    host.appendChild(toast);
    requestAnimationFrame(() => toast.classList.add("is-visible"));
    const remove = () => {
      toast.classList.remove("is-visible");
      setTimeout(() => toast.remove(), 260);
    };
    toast.querySelector("button").addEventListener("click", remove);
    setTimeout(remove, 3600);
  }
  function adCard(ad) {
    const id = ad.advertisementID;
    const image = mediaUrl(ad.thumbnailPath, window.AppConfig.defaultAdvertisementImage);
    const title = escapeHtml(ad.title);
    const category = escapeHtml(ad.categoryName);
    const location = escapeHtml([ad.provinceName, ad.cityName].filter(Boolean).join("، "));
    return `<div class="col-12 col-sm-6 col-lg-4"><article class="ad-card">
      <a class="ad-card__main" href="advertisement-details.html?id=${encodeURIComponent(id)}">
        <div class="ad-card__image-wrap"><img src="${escapeHtml(image)}" alt="${title}" loading="lazy" onerror="this.src='${window.AppConfig.defaultAdvertisementImage}'">${ad.isImmediate ? '<span class="urgent-badge">فوری</span>' : ''}</div>
        <div class="ad-card__body"><span class="ad-card__category">${category}</span><h3>${title}</h3><div class="ad-card__location">${icon("pin", "#ff8a1f")}<span>${location}</span></div><div class="ad-card__price">${formatPrice(ad.price)}</div><div class="ad-card__footer"><span>${escapeHtml(ad.createDatePersian || "")}</span><button class="favorite-btn" data-favorite-id="${id}" type="button" aria-label="ذخیره آگهی">${icon("heart", "currentColor")}</button></div></div>
      </a></article></div>`;
  }
  function applySeo(data) {
    if (!data) return;
    if (data.seoTitle) document.title = `${data.seoTitle} | ${window.AppConfig.siteName}`;
    const meta = document.querySelector('meta[name="description"]');
    if (meta && data.seoDescription) meta.setAttribute("content", data.seoDescription);
    if (data.canonicalUrl) {
      let link = document.querySelector('link[rel="canonical"]');
      if (!link) { link = document.createElement("link"); link.rel = "canonical"; document.head.appendChild(link); }
      link.href = data.canonicalUrl;
    }
  }
  return { escapeHtml, mediaUrl, formatPrice, icon, categoryVisual, loadingMarkup, emptyMarkup, errorMarkup, showToast, adCard, applySeo };
})();
