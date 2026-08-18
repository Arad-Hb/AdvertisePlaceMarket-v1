window.PanelLayout = (function () {
  const cfg = window.AppConfig;
  let currentRole = document.body.dataset.panelRole || "Customer";
  let currentPage = document.body.dataset.panelPage || "dashboard";

  const customerItems = [
    ["dashboard","داشبورد","dashboard","#ff9800","index.html"],
    ["advertisements","آگهی‌های من","ads","#2196f3","advertisements.html"],
    ["create","ثبت آگهی جدید","add","#28c76f","create-advertisement.html"],
    ["favorites","علاقه‌مندی‌ها","heart","#ff2f7d","favorites.html"],
    ["membership","طرح‌های اشتراک","diamond","#8b45e6","membership.html"],
    ["payments","تراکنش‌ها","card","#14a9c9","payments.html"],
    ["profile","پروفایل کاربری","user","#ff9800","profile.html"],
    ["settings","تنظیمات","gear","#a5b1bf","settings.html"]
  ];
  const adminItems = [
    ["dashboard","داشبورد مدیریت","dashboard","#ff9800","index.html"],
    ["advertisements","مدیریت آگهی‌ها","ads","#2196f3","advertisements.html"],
    ["categories","دسته‌بندی‌ها","folder","#00a8e8","categories.html"],
    ["customers","کاربران","users","#ae3de1","customers.html"],
    ["provinces","استان‌ها","map","#24bd77","provinces.html"],
    ["cities","شهرها","globe","#18b6c9","cities.html"],
    ["memberships","تعرفه‌ها و عضویت","diamond","#8b45e6","membership-plans.html"],
    ["payments","تراکنش‌ها","card","#14a9c9","payments.html"],
    ["banners","بنرهای صفحه اصلی","image","#ff6b3d","hero-banners.html"],
    ["site","تنظیمات سایت","gear","#a5b1bf","site-settings.html"],
    ["account","حساب مدیر","user","#ff9800","account.html"]
  ];

  function userName(user) { return [user?.firstName,user?.lastName].filter(Boolean).join(" ") || (currentRole === "Admin" ? "مدیر سایت" : "نام مشتری"); }
  function userAvatar(user) { return window.UI.mediaUrl(user?.avatarPath, cfg.defaultAvatar); }
  function menuItems() { return currentRole === "Admin" ? adminItems : customerItems; }

  function renderSidebar(user) {
    const host = document.getElementById("panelSidebarRoot");
    if (!host) return;
    const roleLabel = currentRole === "Admin" ? "مدیر" : "مشتری";
    const items = menuItems().map(item => `<a class="panel-menu__item ${item[0]===currentPage?'active':''}" href="${item[4]}" title="${item[1]}"><span class="panel-menu__icon">${window.PanelUI.icon(item[2],item[3])}</span><span class="panel-menu__label">${item[1]}</span></a>`).join("");
    host.innerHTML = `<aside class="panel-sidebar" id="panelSidebar">
      <div class="panel-sidebar__user"><img class="panel-sidebar__avatar" src="${window.UI.escapeHtml(userAvatar(user))}" alt=""><div class="panel-sidebar__identity"><div class="panel-sidebar__name">${window.UI.escapeHtml(userName(user))}</div><div class="panel-sidebar__role"><span class="panel-sidebar__online"></span><span>${roleLabel}</span></div></div></div>
      <div class="panel-sidebar__section-title">عمومی</div><nav class="panel-menu">${items}<div class="panel-menu__separator"></div><a class="panel-menu__item" href="${cfg.pageUrl('index.html')}" title="بازگشت به فروشگاه"><span class="panel-menu__icon">${window.PanelUI.icon('home','#0d6efd')}</span><span class="panel-menu__label">بازگشت به فروشگاه</span></a><button class="panel-menu__item w-100 border-0 bg-transparent text-start" id="panelLogout" type="button" title="خروج"><span class="panel-menu__icon">${window.PanelUI.icon('logout','#ff3b3b')}</span><span class="panel-menu__label">خروج</span></button></nav>
    </aside>`;
  }

  function renderHeader(user) {
    const host = document.getElementById("panelHeaderRoot");
    if (!host) return;
    const profileUrl = currentRole === "Admin" ? "account.html" : "profile.html";
    const settingsUrl = currentRole === "Admin" ? "account.html" : "settings.html";
    host.innerHTML = `<header class="panel-header" id="panelHeader"><button class="panel-header__toggle" id="sidebarToggle" type="button" aria-label="باز و بسته کردن سایدبار">${window.PanelUI.icon('menu','#fff')}</button><div class="panel-header__inner">
      <div class="d-flex align-items-center gap-3 min-w-0"><a class="panel-header__brand" href="${cfg.pageUrl('index.html')}"><img src="${cfg.brandLogo}" alt="تحلیل داده"><strong>تحلیل داده آگهی</strong></a><a class="panel-header__back" href="${cfg.pageUrl('index.html')}">${window.PanelUI.icon('home','#fff')}<span>بازگشت به فروشگاه</span></a></div>
      <div class="panel-header__left"><div class="dropdown panel-user-dropdown"><button class="panel-user-dropdown__button" type="button" data-bs-toggle="dropdown" aria-expanded="false"><img src="${window.UI.escapeHtml(userAvatar(user))}" alt=""><span>${window.UI.escapeHtml(userName(user))}</span><b aria-hidden="true">${window.PanelUI.icon('chevronDown')}</b></button><ul class="dropdown-menu dropdown-menu-start"><li><a class="dropdown-item" href="${profileUrl}">${window.PanelUI.icon('user','#516170')}<span>پروفایل من</span></a></li><li><a class="dropdown-item" href="${settingsUrl}">${window.PanelUI.icon('gear','#516170')}<span>تنظیمات</span></a></li><li><a class="dropdown-item" href="${cfg.pageUrl('index.html')}">${window.PanelUI.icon('home','#0d6efd')}<span>بازگشت به فروشگاه</span></a></li><li><hr class="dropdown-divider"></li><li><button class="dropdown-item text-danger" id="headerLogout" type="button">${window.PanelUI.icon('logout','#dc3545')}<span>خروج</span></button></li></ul></div></div>
    </div></header>`;
  }

  function renderFooter() {
    const host = document.getElementById("panelFooterRoot");
    if (!host) return;
    host.innerHTML = `<footer class="panel-footer"><span>© تحلیل داده آگهی — همه حقوق محفوظ است.</span><a href="${cfg.pageUrl('index.html')}">بازگشت به فروشگاه</a></footer>`;
  }

  function applySidebarState() {
    const sidebar = document.getElementById("panelSidebar");
    if (!sidebar || innerWidth < 992) return;
    if (localStorage.getItem(cfg.sidebarStorageKey) === "1") sidebar.classList.add("is-collapsed");
  }

  function bind() {
    const sidebar = document.getElementById("panelSidebar");
    const toggle = document.getElementById("sidebarToggle");
    let overlay = document.getElementById("panelOverlay");
    if (!overlay) { overlay = document.createElement("div"); overlay.id = "panelOverlay"; overlay.className = "panel-overlay"; document.body.appendChild(overlay); }

    function closeMobile() { sidebar?.classList.remove("is-mobile-open"); overlay.classList.remove("is-visible"); }
    toggle?.addEventListener("click", () => {
      if (innerWidth < 992) { const open = !sidebar.classList.contains("is-mobile-open"); sidebar.classList.toggle("is-mobile-open",open); overlay.classList.toggle("is-visible",open); }
      else { sidebar.classList.toggle("is-collapsed"); localStorage.setItem(cfg.sidebarStorageKey, sidebar.classList.contains("is-collapsed") ? "1" : "0"); }
    });
    overlay.addEventListener("click",closeMobile);
    addEventListener("resize",() => { if(innerWidth>=992) closeMobile(); });
    document.getElementById("panelLogout")?.addEventListener("click", () => window.Auth.logout());
    document.getElementById("headerLogout")?.addEventListener("click", () => window.Auth.logout());
  }

  async function init() {
    if (!window.Auth.requireRole(currentRole)) return null;
    const user = await window.Auth.loadAuthenticatedUser();
    if (!user || !window.Auth.hasRole(currentRole)) { window.Auth.clear(); window.Auth.redirectToLogin(location.href); return null; }
    renderSidebar(user); renderHeader(user); renderFooter(); applySidebarState(); bind();
    document.querySelectorAll('[data-current-user-name]').forEach(el => el.textContent = userName(user));
    document.querySelectorAll('[data-current-user-avatar]').forEach(el => el.src = userAvatar(user));
    return user;
  }

  return { init, userName, userAvatar };
})();
