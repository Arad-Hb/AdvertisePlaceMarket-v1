(async function () {
  if (!await window.PanelLayout.init()) return;
  const host = document.getElementById("adminPaymentsHost");
  const pager = document.getElementById("adminPaymentsPagination");
  const kw = document.getElementById("adminPaymentKeyword");
  const from = document.getElementById("adminPaymentFrom");
  const to = document.getElementById("adminPaymentTo");
  let page = 1;
  let currentItems = [];

  function paymentRow(p) {
    return `<tr><td><strong>${window.UI.escapeHtml(p.customerName)}</strong><div class="small text-muted">${window.UI.escapeHtml(p.mobileNumber)}</div></td><td>${window.UI.escapeHtml(p.membershipPlanTitle)}</td><td>${window.UI.formatPrice(p.amount)}</td><td>${window.UI.escapeHtml(p.trackingCode || "-")}</td><td>${p.isPaid ? '<span class="payment-state paid">موفق</span>' : '<span class="payment-state unpaid">ناموفق</span>'}</td><td>${window.UI.escapeHtml(p.createDatePersian || "")}</td><td>${window.UI.escapeHtml(p.paidDatePersian || "-")}</td><td><div class="panel-actions">${window.PanelUI.actionBtn(`data-view="${p.paymentID}"`, "eye", "#214162", "جزئیات")}</div></td></tr>`;
  }

  function showDetails(id) {
    const p = currentItems.find((x) => String(x.paymentID) === String(id));
    if (!p) return;
    window.PanelUI.openDetailsModal("جزئیات تراکنش", `${window.PanelUI.detailsGrid([
      ["مشتری", window.UI.escapeHtml(p.customerName)],
      ["موبایل", window.UI.escapeHtml(p.mobileNumber)],
      ["طرح", window.UI.escapeHtml(p.membershipPlanTitle)],
      ["مبلغ", window.UI.formatPrice(p.amount)],
      ["کد پیگیری", window.UI.escapeHtml(p.trackingCode || "-")],
      ["وضعیت", p.isPaid ? '<span class="payment-state paid">موفق</span>' : '<span class="payment-state unpaid">ناموفق</span>'],
      ["تاریخ ایجاد", window.UI.escapeHtml(p.createDatePersian || "-")],
      ["تاریخ پرداخت", window.UI.escapeHtml(p.paidDatePersian || "-")]
    ])}`);
  }

  async function load() {
    host.innerHTML = window.PanelUI.loading();
    const params = { PageIndex: page, PageSize: 10 };
    if (kw.value.trim()) params.CustomerKeyword = kw.value.trim();
    if (from.value) params.FromDate = from.value;
    if (to.value) params.ToDate = to.value;
    try {
      const r = await window.Api.get(window.AppConfig.endpoints.adminPayments, { params });
      const items = window.PanelUI.items(r.data);
      const pm = window.PanelUI.pageModel(r.data);
      currentItems = items;
      host.innerHTML = items.length ? `<section class="panel-card"><div class="panel-table-wrap"><table class="panel-table"><thead><tr><th>مشتری</th><th>طرح</th><th>مبلغ</th><th>کد پیگیری</th><th>وضعیت</th><th>تاریخ ایجاد</th><th>تاریخ پرداخت</th><th>عملیات</th></tr></thead><tbody>${items.map(paymentRow).join("")}</tbody></table></div></section>` : window.PanelUI.empty();
      host.querySelectorAll("[data-view]").forEach((b) => b.addEventListener("click", () => showDetails(b.dataset.view)));
      window.PanelUI.renderPagination(pager, pm, (p) => { page = p; load(); });
    } catch (e) {
      host.innerHTML = window.PanelUI.error(window.Api.normalizeError(e).message);
    }
  }

  document.getElementById("adminPaymentFilter").addEventListener("click", () => { page = 1; load(); });
  document.getElementById("adminPaymentClear").addEventListener("click", () => { kw.value = ""; from.value = ""; to.value = ""; page = 1; load(); });
  load();
})();
