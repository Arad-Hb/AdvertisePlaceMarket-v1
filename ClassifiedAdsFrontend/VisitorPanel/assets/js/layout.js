(function () {
  const headerRoot = document.getElementById("siteHeaderRoot");
  const footerRoot = document.getElementById("siteFooterRoot");
  if (!headerRoot && !footerRoot) return;

  function currentPage() { return document.body.dataset.page || ""; }
  function activeClass(name) { return currentPage() === name ? "active" : ""; }

  function renderHeader() {
    if (!headerRoot) return;
    headerRoot.innerHTML = `<header class="site-header"><nav class="site-nav"><div class="container site-nav__inner">
      <button class="mobile-burger" type="button" data-bs-toggle="offcanvas" data-bs-target="#mobileMenu" aria-label="باز کردن منو">${window.UI.icon("menu", "#fff")}</button>
      <a class="site-brand" href="index.html"><img src="assets/images/logo/tahlildadeh-logo.png" alt="تحلیل داده آگهی"><span>تحلیل داده آگهی</span></a>
      <div class="desktop-links">
        <a class="${activeClass("home")}" href="index.html">خانه</a>
        <div class="mega-wrap"><button type="button" id="megaToggle">دسته‌بندی‌ها <span>⌄</span></button><div class="mega-menu" id="megaMenu"><div class="mega-menu__head"><strong>دسته‌بندی آگهی‌ها</strong><a href="categories.html">مشاهده همه</a></div><div class="mega-menu__grid" id="megaMenuContent">${window.UI.loadingMarkup()}</div></div></div>
        <a class="${activeClass("ads")}" href="advertisements.html">آگهی‌ها</a>
        <a class="${activeClass("pricing")}" href="pricing.html">تعرفه‌ها</a>
      </div>
      <form class="header-search header-search--desktop" data-header-search><input name="keyword" type="search" placeholder="جستجو"><button aria-label="جستجو">${window.UI.icon("search", "#ff8a1f")}</button></form>
      <form class="header-search header-search--mobile" data-header-search><input name="keyword" type="search" placeholder="جستجو"><button aria-label="جستجو">${window.UI.icon("search", "#ff8a1f")}</button></form>
      <div class="header-actions" id="headerUserArea"><a class="header-auth" href="login.html" aria-label="ورود یا ثبت نام">${window.UI.icon("user", "#fff")}<span>ورود / ثبت‌نام</span></a><a class="btn header-post" href="login.html?returnUrl=customer/create-advertisement.html">+ ثبت آگهی</a></div>
      <a class="mobile-auth" href="login.html" aria-label="ورود یا ثبت نام">${window.UI.icon("user", "#fff")}</a>
    </div></nav></header>
    <div class="offcanvas offcanvas-end mobile-offcanvas" tabindex="-1" id="mobileMenu"><div class="offcanvas-header"><strong>منوی سایت</strong><button type="button" class="btn-close btn-close-white" data-bs-dismiss="offcanvas" aria-label="بستن"></button></div><div class="offcanvas-body"><a href="index.html">خانه</a><a href="advertisements.html">آگهی‌ها</a><a href="categories.html">دسته‌بندی‌ها</a><a href="pricing.html">تعرفه‌ها</a><div class="mobile-category-list" id="mobileCategoryList">${window.UI.loadingMarkup()}</div><a class="mobile-post-link" href="login.html?returnUrl=customer/create-advertisement.html">+ ثبت آگهی</a></div></div>`;
  }

  function renderFooter() {
    if (!footerRoot) return;
    footerRoot.innerHTML = `<footer class="site-footer"><div class="container"><div class="footer-grid">
      <section class="footer-brand"><img src="assets/images/logo/tahlildadeh-logo.png" alt="تحلیل داده آگهی"><div><h2>تحلیل داده آگهی</h2><p id="footerDescription">سامانه‌ای ساده برای جستجو، مشاهده و ثبت آگهی‌های آنلاین.</p></div></section>
      <section><h3>دسترسی سریع</h3><nav class="footer-links"><a href="index.html">خانه</a><a href="categories.html">دسته‌بندی‌ها</a><a href="advertisements.html">آگهی‌ها</a><a href="pricing.html">تعرفه‌ها</a></nav></section>
      <section><h3>درباره، قوانین و حریم خصوصی</h3><p class="footer-summary">استفاده از این سامانه به معنی پذیرش قوانین انتشار آگهی، رعایت حقوق کاربران و حفظ اطلاعات شخصی مطابق سیاست‌های سایت است. مسئولیت صحت محتوای هر آگهی بر عهده ثبت‌کننده آن است.</p><div class="footer-socials" id="footerSocials"></div></section>
    </div><div class="footer-bottom"><span>© ۱۴۰۵ تحلیل داده آگهی — همه حقوق محفوظ است.</span></div></div></footer>`;
  }

  function bindHeaderSearch() {
    document.querySelectorAll("[data-header-search]").forEach(form => form.addEventListener("submit", event => {
      event.preventDefault();
      const keyword = new FormData(form).get("keyword");
      location.href = `advertisements.html${keyword && String(keyword).trim() ? `?Keyword=${encodeURIComponent(String(keyword).trim())}` : ""}`;
    }));
  }

  function bindMega() {
    const toggle = document.getElementById("megaToggle");
    const menu = document.getElementById("megaMenu");
    if (!toggle || !menu) return;
    const close = () => { menu.classList.remove("is-open"); toggle.setAttribute("aria-expanded", "false"); };
    toggle.addEventListener("click", event => { event.stopPropagation(); menu.classList.toggle("is-open"); toggle.setAttribute("aria-expanded", String(menu.classList.contains("is-open"))); });
    document.addEventListener("click", event => { if (!event.target.closest(".mega-wrap")) close(); });
    document.addEventListener("keydown", event => { if (event.key === "Escape") close(); });
  }

  async function loadNavData() {
    try {
      const categories = await window.Categories.getMenu();
      window.Categories.renderMegaMenu(categories);
      window.Categories.renderMobileMenu(categories);
    } catch (error) {
      document.getElementById("megaMenuContent").innerHTML = window.UI.errorMarkup(window.Api.normalizeError(error).message);
      document.getElementById("mobileCategoryList").innerHTML = "";
    }
  }

  async function loadUser() {
    const host = document.getElementById("headerUserArea");
    if (!host || !window.Auth.isAuthenticated()) return;
    const user = await window.Auth.loadAuthenticatedUser();
    if (!user) return;
    const name = `${user.firstName || ""} ${user.lastName || ""}`.trim() || user.mobileNumber;
    const avatar = window.UI.mediaUrl(user.avatarPath, window.AppConfig.defaultAvatar);
    host.innerHTML = `<div class="header-user dropdown"><button class="header-user__button" data-bs-toggle="dropdown" aria-expanded="false"><img src="${window.UI.escapeHtml(avatar)}" alt=""><span>${window.UI.escapeHtml(name)}</span><b>⌄</b></button><ul class="dropdown-menu dropdown-menu-end"><li><a class="dropdown-item" href="${window.Auth.dashboardUrl()}">${window.Auth.hasRole("Admin") ? "مدیریت سایت" : "پنل کاربری"}</a></li>${window.Auth.hasRole("Customer") ? '<li><a class="dropdown-item" href="customer/favorites.html">علاقه‌مندی‌ها</a></li>' : ''}<li><button class="dropdown-item text-danger" id="logoutButton">خروج</button></li></ul></div><a class="btn header-post" href="${window.Auth.hasRole("Customer") ? "customer/create-advertisement.html" : "admin/index.html"}">+ ثبت آگهی</a>`;
    document.getElementById("logoutButton")?.addEventListener("click", () => window.Auth.logout());
    const mobileAuth = document.querySelector(".mobile-auth");
    if (mobileAuth) { mobileAuth.href = window.Auth.dashboardUrl(); mobileAuth.innerHTML = `<img src="${window.UI.escapeHtml(avatar)}" alt="${window.UI.escapeHtml(name)}">`; }
  }

  async function loadFooterSettings() {
    try {
      const settings = await window.SiteData.getSettings();
      if (settings.siteDescription) document.getElementById("footerDescription").textContent = settings.siteDescription;
      const socials = document.getElementById("footerSocials");
      const links = [
        [settings.telegramUrl, "TG"], [settings.instagramUrl, "IG"], [settings.linkedInUrl, "in"]
      ].filter(x => x[0]);
      socials.innerHTML = links.map(x => `<a href="${window.UI.escapeHtml(x[0])}" target="_blank" rel="noopener">${x[1]}</a>`).join("");
    } catch { /* footer remains useful with defaults */ }
  }

  renderHeader(); renderFooter(); bindHeaderSearch(); bindMega(); loadNavData(); loadUser(); loadFooterSettings();
})();
