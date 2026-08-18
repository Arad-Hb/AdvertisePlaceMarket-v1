(async function () {
  if (!await window.PanelLayout.init()) return;
  const host = document.getElementById("customersHost");
  const pager = document.getElementById("customersPagination");
  const keyword = document.getElementById("customerKeyword");
  const active = document.getElementById("customerActive");
  const modalRoot = document.getElementById("customerModalRoot");
  let page = 1;

  function fullName(u) {
    return [u.firstName, u.lastName].filter(Boolean).join(" ") || "بدون نام";
  }

  async function load() {
    host.innerHTML = window.PanelUI.loading();
    const params = { PageIndex: page, PageSize: 10 };
    if (keyword.value.trim()) params.Keyword = keyword.value.trim();
    if (active.value !== "") params.IsActive = active.value === "true";
    try {
      const r = await window.Api.get(window.AppConfig.endpoints.adminCustomers, { params });
      const items = window.PanelUI.items(r.data);
      const pm = window.PanelUI.pageModel(r.data);
      host.innerHTML = items.length ? `<section class="panel-card"><div class="panel-table-wrap"><table class="panel-table"><thead><tr><th>کاربر</th><th>شماره موبایل</th><th>تاریخ عضویت</th><th>وضعیت</th><th>عملیات</th></tr></thead><tbody>${items.map((u) => {
        const id = window.UI.escapeHtml(u.userID);
        const toggleTitle = u.isActive ? "غیرفعال کردن" : "فعال کردن";
        const toggleIcon = u.isActive
          ? window.PanelUI.actionBtn(`data-toggle="${id}" data-active="${u.isActive}"`, "close", "#ef3d45", toggleTitle)
          : window.PanelUI.actionBtn(`data-toggle="${id}" data-active="${u.isActive}"`, "check", "#18ad55", toggleTitle);
        return `<tr><td><div class="d-flex align-items-center gap-2"><img class="panel-avatar-sm" src="${window.UI.escapeHtml(window.UI.mediaUrl(u.avatarPath, window.AppConfig.defaultAvatar))}" alt=""><strong>${window.UI.escapeHtml(fullName(u))}</strong></div></td><td>${window.UI.escapeHtml(u.mobileNumber)}</td><td>${window.UI.escapeHtml(u.createDatePersian || "")}</td><td><span class="toggle-state ${u.isActive ? "" : "off"}"><i></i>${u.isActive ? "فعال" : "غیرفعال"}</span></td><td><div class="panel-actions">${window.PanelUI.actionBtn(`data-view="${id}"`, "eye", "#214162", "جزئیات")}${window.PanelUI.actionBtn(`data-edit="${id}"`, "edit", "#0d6efd", "ویرایش")}${toggleIcon}${window.PanelUI.actionBtn(`data-delete="${id}"`, "trash", "#ef3d45", "حذف")}</div></td></tr>`;
      }).join("")}</tbody></table></div></section>` : window.PanelUI.empty();
      bind();
      window.PanelUI.renderPagination(pager, pm, (p) => { page = p; load(); });
    } catch (e) {
      host.innerHTML = window.PanelUI.error(window.Api.normalizeError(e).message);
    }
  }

  function openForm(d) {
    const isEdit = Boolean(d && d.userID);
    modalRoot.innerHTML = `<div class="modal fade panel-modal" id="customerModal" tabindex="-1"><div class="modal-dialog"><div class="modal-content"><form id="customerForm" class="panel-form"><div class="modal-header"><h5 class="modal-title">${isEdit ? "ویرایش کاربر" : "کاربر جدید"}</h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div><div class="modal-body"><div class="row g-3"><div class="col-md-6"><label class="form-label">نام *</label><input class="form-control" name="firstName" maxlength="50" required value="${window.UI.escapeHtml(d.firstName || "")}"></div><div class="col-md-6"><label class="form-label">نام خانوادگی *</label><input class="form-control" name="lastName" maxlength="80" required value="${window.UI.escapeHtml(d.lastName || "")}"></div><div class="col-md-6"><label class="form-label">شماره موبایل *</label><input class="form-control" name="mobileNumber" required pattern="^09\\d{9}$" maxlength="11" value="${window.UI.escapeHtml(d.mobileNumber || "")}" placeholder="0912xxxxxxx"></div><div class="col-md-6"><label class="form-label">ایمیل</label><input class="form-control" type="email" name="email" value="${window.UI.escapeHtml(d.email || "")}"></div><div class="col-md-6"><label class="form-label">${isEdit ? "رمز عبور جدید" : "رمز عبور *"}</label><input class="form-control" type="password" name="password" minlength="6" ${isEdit ? "" : "required"} autocomplete="new-password"></div><div class="col-md-6"><label class="form-label">${isEdit ? "تکرار رمز جدید" : "تکرار رمز عبور *"}</label><input class="form-control" type="password" name="confirmPassword" minlength="6" ${isEdit ? "" : "required"} autocomplete="new-password"></div><div class="col-12"><label class="panel-switch"><input class="form-check-input" type="checkbox" name="isActive" ${d.isActive === false ? "" : "checked"}><span>حساب فعال باشد</span></label></div><div class="col-12 small text-muted">${isEdit ? "اگر رمز را خالی بگذارید، رمز فعلی تغییر نمی‌کند." : "رمز باید حداقل ۶ کاراکتر و شامل عدد باشد."}</div></div></div><div class="modal-footer"><button class="btn-panel-primary" type="submit">ذخیره</button></div></form></div></div></div>`;
    const instance = new bootstrap.Modal(document.getElementById("customerModal"));
    instance.show();
    document.getElementById("customerForm").addEventListener("submit", async (e) => {
      e.preventDefault();
      const x = window.PanelUI.serializeForm(e.currentTarget);
      if ((x.password || x.confirmPassword) && x.password !== x.confirmPassword) {
        window.UI.showToast("رمز عبور و تکرار آن یکسان نیستند.", "error");
        return;
      }
      if (!isEdit && !x.password) {
        window.UI.showToast("رمز عبور الزامی است.", "error");
        return;
      }
      if (!x.password) {
        delete x.password;
        delete x.confirmPassword;
      }
      try {
        const r = isEdit
          ? await window.Api.put(window.AppConfig.endpoints.adminCustomer(d.userID), x)
          : await window.Api.post(window.AppConfig.endpoints.adminCustomers, x);
        window.UI.showToast(window.PanelUI.opMessage(r), "success");
        instance.hide();
        load();
      } catch (err) {
        window.UI.showToast(window.Api.normalizeError(err).message, "error");
      }
    });
  }

  async function toggleUser(userId, currentlyActive) {
    if (!confirm(currentlyActive ? "کاربر غیرفعال شود؟" : "کاربر فعال شود؟")) return;
    try {
      const url = currentlyActive ? window.AppConfig.endpoints.adminDeactivateCustomer(userId) : window.AppConfig.endpoints.adminActivateCustomer(userId);
      const r = await window.Api.patch(url, {});
      window.UI.showToast(window.PanelUI.opMessage(r), "success");
      load();
    } catch (e) {
      window.UI.showToast(window.Api.normalizeError(e).message, "error");
    }
  }

  async function deleteUser(userId) {
    if (!confirm("این کاربر حذف شود؟ آگهی‌ها، تراکنش‌ها و عضویت او هم حذف می‌شوند.")) return;
    try {
      const r = await window.Api.delete(window.AppConfig.endpoints.adminCustomer(userId));
      window.UI.showToast(window.PanelUI.opMessage(r), "success");
      load();
    } catch (e) {
      window.UI.showToast(window.Api.normalizeError(e).message, "error");
    }
  }

  async function showDetails(userId) {
    window.PanelUI.openDetailsModal("جزئیات کاربر", window.PanelUI.loading(), "modal-xl");
    const body = document.querySelector("#panelDetailsModal .modal-body");
    try {
      const u = (await window.Api.get(window.AppConfig.endpoints.adminCustomer(userId))).data;
      const name = fullName(u);
      const [adsRes, payRes] = await Promise.all([
        window.Api.get(window.AppConfig.endpoints.adminAdvertisements, { params: { CustomerUserID: u.userID, PageIndex: 1, PageSize: 8, Sort: "newest" } }),
        window.Api.get(window.AppConfig.endpoints.adminPayments, { params: { CustomerKeyword: u.mobileNumber, PageIndex: 1, PageSize: 5 } })
      ]);
      const ads = window.PanelUI.items(adsRes.data);
      const payments = window.PanelUI.items(payRes.data);
      const adsHtml = ads.length
        ? `<div class="panel-detail-list">${ads.map((a) => `<div class="panel-detail-list__item"><img src="${window.UI.escapeHtml(window.UI.mediaUrl(a.thumbnailPath))}" alt=""><div><strong>${window.UI.escapeHtml(a.title)}</strong><div class="small text-muted">${window.UI.escapeHtml(a.categoryName || "")} · ${window.UI.escapeHtml(a.createDatePersian || "")}</div></div><div class="d-flex align-items-center gap-2">${window.PanelUI.status(a.statusCode, a.statusTitle)}${window.PanelUI.actionBtn(`data-admin-ad-review="${a.advertisementID}"`, "eye", "#214162", "بررسی آگهی")}</div></div>`).join("")}</div>`
        : '<div class="text-muted small">آگهی‌ای ثبت نشده است.</div>';
      const payHtml = payments.length
        ? `<div class="panel-detail-list">${payments.map((p) => `<div class="panel-detail-list__item"><div><strong>${window.UI.escapeHtml(p.membershipPlanTitle)}</strong><div class="small text-muted">${window.UI.escapeHtml(p.createDatePersian || "")} · ${window.UI.escapeHtml(p.trackingCode || "-")}</div></div><div><strong>${window.UI.formatPrice(p.amount)}</strong> ${p.isPaid ? '<span class="payment-state paid">موفق</span>' : '<span class="payment-state unpaid">ناموفق</span>'}</div></div>`).join("")}</div>`
        : '<div class="text-muted small">تراکنشی ثبت نشده است.</div>';
      body.innerHTML = `<div class="d-flex align-items-center gap-3 mb-3"><img class="panel-avatar-sm" style="width:56px;height:56px" src="${window.UI.escapeHtml(window.UI.mediaUrl(u.avatarPath, window.AppConfig.defaultAvatar))}" alt=""><div><div class="fw-bold">${window.UI.escapeHtml(name)}</div><div class="small text-muted">${window.UI.escapeHtml(u.mobileNumber)}</div></div></div>${window.PanelUI.detailsGrid([
        ["نام", window.UI.escapeHtml(name)],
        ["موبایل", window.UI.escapeHtml(u.mobileNumber)],
        ["ایمیل", window.UI.escapeHtml(u.email || "-")],
        ["وضعیت", `<span class="toggle-state ${u.isActive ? "" : "off"}"><i></i>${u.isActive ? "فعال" : "غیرفعال"}</span>`],
        ["تاریخ عضویت", window.UI.escapeHtml(u.createDatePersian || "-")]
      ])}<div class="panel-detail-split mt-4"><section><h3 class="panel-detail-split__title">آخرین آگهی‌ها</h3>${adsHtml}</section><section><h3 class="panel-detail-split__title">آخرین تراکنش‌ها</h3>${payHtml}</section></div><div class="review-actions mt-3"><button type="button" class="btn-panel-primary" data-modal-edit>ویرایش</button>${u.isActive ? '<button type="button" class="btn-panel-danger" data-modal-toggle>غیرفعال کردن کاربر</button>' : '<button type="button" class="btn-panel-success" data-modal-toggle>فعال کردن کاربر</button>'}</div>`;
      body.querySelector("[data-modal-edit]")?.addEventListener("click", () => {
        bootstrap.Modal.getInstance(document.getElementById("panelDetailsModal"))?.hide();
        openForm(u);
      });
      body.querySelector("[data-modal-toggle]")?.addEventListener("click", () => {
        bootstrap.Modal.getInstance(document.getElementById("panelDetailsModal"))?.hide();
        toggleUser(u.userID, u.isActive);
      });
    } catch (e) {
      body.innerHTML = window.PanelUI.error(window.Api.normalizeError(e).message);
    }
  }

  function bind() {
    host.querySelectorAll("[data-view]").forEach((b) => b.addEventListener("click", () => showDetails(b.dataset.view)));
    host.querySelectorAll("[data-edit]").forEach((b) => b.addEventListener("click", async () => {
      try {
        openForm((await window.Api.get(window.AppConfig.endpoints.adminCustomer(b.dataset.edit))).data);
      } catch (e) {
        window.UI.showToast(window.Api.normalizeError(e).message, "error");
      }
    }));
    host.querySelectorAll("[data-toggle]").forEach((b) => b.addEventListener("click", () => toggleUser(b.dataset.toggle, b.dataset.active === "true")));
    host.querySelectorAll("[data-delete]").forEach((b) => b.addEventListener("click", () => deleteUser(b.dataset.delete)));
  }

  document.getElementById("addCustomer").addEventListener("click", () => openForm({ isActive: true }));
  document.getElementById("customerFilter").addEventListener("click", () => { page = 1; load(); });
  document.getElementById("customerClear").addEventListener("click", () => { keyword.value = ""; active.value = ""; page = 1; load(); });
  load();
})();
