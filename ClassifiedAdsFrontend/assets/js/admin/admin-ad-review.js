window.AdminAdReview = (function () {
  let modalRoot = null;
  const refreshHandlers = {};

  function registerRefresh(key, fn) {
    if (typeof fn === "function") refreshHandlers[key] = fn;
  }

  function ensureRoot() {
    if (!modalRoot) {
      modalRoot = document.createElement("div");
      modalRoot.id = "adminAdReviewModalRoot";
      document.body.appendChild(modalRoot);
    }
    return modalRoot;
  }

  function renderContent(a) {
    const imgs = a.images || [];
    const main = imgs.find((x) => x.isMainImage) || imgs[0];
    return `<div class="review-layout review-layout--modal"><section class="panel-card"><div class="panel-card__body"><img class="review-gallery-main" src="${window.UI.escapeHtml(window.UI.mediaUrl(main?.imagePath))}" alt=""><div class="review-thumbs">${imgs.map((i) => `<img src="${window.UI.escapeHtml(window.UI.mediaUrl(i.thumbnailPath || i.imagePath))}" alt="">`).join("")}</div><h2 class="h4 mt-4">${window.UI.escapeHtml(a.title)}</h2><div class="mt-2">${window.PanelUI.status(a.statusCode, a.statusTitle)} ${a.isImmediate ? '<span class="status-pill" style="background:#eaf2ff;color:#0d6efd">فوری</span>' : ""}</div><p class="mt-4 text-secondary" style="white-space:pre-line">${window.UI.escapeHtml(a.description)}</p></div></section><aside><section class="panel-card"><div class="panel-card__header"><h3>اطلاعات آگهی</h3></div><div class="panel-card__body"><div class="review-details"><div class="review-detail"><span>قیمت</span><strong>${window.UI.formatPrice(a.price)}</strong></div><div class="review-detail"><span>شماره تماس</span><strong>${window.UI.escapeHtml(a.phoneNumber)}</strong></div><div class="review-detail"><span>دسته‌بندی</span><strong>${window.UI.escapeHtml(a.categoryName)}</strong></div><div class="review-detail"><span>مشتری</span><strong>${window.UI.escapeHtml(a.customerName || "-")}</strong></div><div class="review-detail"><span>مکان</span><strong>${window.UI.escapeHtml(a.provinceName)}، ${window.UI.escapeHtml(a.cityName)}</strong></div><div class="review-detail"><span>تاریخ ثبت</span><strong>${window.UI.escapeHtml(a.createDatePersian || "")}</strong></div><div class="review-detail"><span>بازدید</span><strong>${Number(a.viewCount || 0).toLocaleString("fa-IR")}</strong></div></div>${a.rejectionReason ? `<div class="rejection-box mt-3">دلیل رد قبلی: ${window.UI.escapeHtml(a.rejectionReason)}</div>` : ""}<div class="review-actions">${a.statusCode === "Pending" ? `<button class="btn-panel-success" data-action="approve">تأیید آگهی</button><button class="btn-panel-danger" data-action="reject">رد آگهی</button>` : ""}${["Pending", "Published"].includes(a.statusCode) ? `<button class="btn-panel-soft" data-action="disable">غیرفعال‌سازی</button>` : ""}${a.statusCode === "Published" ? `<a class="btn-panel-primary" href="../advertisement-details.html?id=${a.advertisementID}" target="_blank" rel="noopener">مشاهده در سایت</a>` : ""}</div></div></section></aside></div>`;
  }

  function rejectModalHtml() {
    return `<div class="modal fade panel-modal" id="adminAdRejectModal" tabindex="-1"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h5 class="modal-title">دلیل رد آگهی</h5><button class="btn-close" data-bs-dismiss="modal"></button></div><div class="modal-body"><textarea class="form-control reject-reason" id="adminAdRejectReason" maxlength="1000" placeholder="دلیل رد را واضح بنویسید..."></textarea></div><div class="modal-footer"><button class="btn-panel-danger" id="adminAdConfirmReject">ثبت رد آگهی</button></div></div></div></div>`;
  }

  async function reloadBody(body, id, onUpdated, refreshList) {
    const r = await window.Api.get(window.AppConfig.endpoints.adminAdvertisement(id));
    const inModal = body.id === "adminAdReviewBody";
    body.innerHTML = renderContent(r.data) + (inModal ? "" : rejectModalHtml());
    bindActions(body, id, onUpdated);
    if (refreshList && typeof onUpdated === "function") onUpdated(r.data);
  }

  function bindRejectConfirm(id, body, onUpdated) {
    const confirmBtn = document.getElementById("adminAdConfirmReject");
    if (!confirmBtn) return;
    confirmBtn.onclick = async () => {
      const reason = document.getElementById("adminAdRejectReason")?.value.trim();
      if (!reason) {
        window.UI.showToast("دلیل رد الزامی است.", "error");
        return;
      }
      try {
        const r = await window.Api.patch(window.AppConfig.endpoints.adminRejectAdvertisement(id), { rejectionReason: reason });
        window.UI.showToast(window.PanelUI.opMessage(r), "success");
        bootstrap.Modal.getInstance(document.getElementById("adminAdRejectModal"))?.hide();
        document.getElementById("adminAdRejectReason").value = "";
        await reloadBody(body, id, onUpdated, true);
      } catch (e) {
        window.UI.showToast(window.Api.normalizeError(e).message, "error");
      }
    };
  }

  function bindActions(host, id, onUpdated) {
    const body = host.closest(".modal-body") || host;
    host.querySelector('[data-action="approve"]')?.addEventListener("click", () => runAction("approve", id, body, onUpdated));
    host.querySelector('[data-action="disable"]')?.addEventListener("click", () => runAction("disable", id, body, onUpdated));
    host.querySelector('[data-action="reject"]')?.addEventListener("click", () => {
      const rejectEl = document.getElementById("adminAdRejectModal");
      if (!rejectEl) return;
      bindRejectConfirm(id, body, onUpdated);
      new bootstrap.Modal(rejectEl).show();
    });
  }

  async function runAction(type, id, body, onUpdated) {
    const text = type === "approve" ? "آگهی تأیید و منتشر شود؟" : "آگهی غیرفعال شود؟";
    if (!confirm(text)) return;
    try {
      const url = type === "approve" ? window.AppConfig.endpoints.adminApproveAdvertisement(id) : window.AppConfig.endpoints.adminDisableAdvertisement(id);
      const r = await window.Api.patch(url, {});
      window.UI.showToast(window.PanelUI.opMessage(r), "success");
      await reloadBody(body, id, onUpdated, true);
    } catch (e) {
      window.UI.showToast(window.Api.normalizeError(e).message, "error");
    }
  }

  async function open(adId, options) {
    const id = Number(adId || 0);
    if (!id) return;
    const onUpdated = options?.onUpdated || (options?.refreshKey ? refreshHandlers[options.refreshKey] : null);
    const root = ensureRoot();
    root.innerHTML = `<div class="modal fade panel-modal" id="adminAdReviewModal" tabindex="-1"><div class="modal-dialog modal-xl modal-dialog-scrollable"><div class="modal-content"><div class="modal-header"><h5 class="modal-title">بررسی آگهی</h5><button class="btn-close" data-bs-dismiss="modal"></button></div><div class="modal-body" id="adminAdReviewBody">${window.PanelUI.loading()}</div></div></div></div>${rejectModalHtml()}`;
    const modalEl = document.getElementById("adminAdReviewModal");
    const body = document.getElementById("adminAdReviewBody");
    const instance = new bootstrap.Modal(modalEl);
    instance.show();
    modalEl.addEventListener("hidden.bs.modal", () => { root.innerHTML = ""; }, { once: true });
    try {
      await reloadBody(body, id, onUpdated);
    } catch (e) {
      body.innerHTML = window.PanelUI.error(window.Api.normalizeError(e).message);
    }
  }

  function renderPage(host, a, id, onReload) {
    host.innerHTML = renderContent(a) + rejectModalHtml();
    bindActions(host, id, onReload);
  }

  document.addEventListener("click", (e) => {
    const btn = e.target.closest("[data-admin-ad-review]");
    if (!btn) return;
    e.preventDefault();
    open(btn.dataset.adminAdReview, { refreshKey: btn.dataset.reviewRefresh || "" });
  });

  return { open, renderPage, registerRefresh };
})();
