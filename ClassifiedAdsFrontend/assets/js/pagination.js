window.Pagination = {
  render: function (host, pageModel, onPage) {
    if (!host) return;
    const current = Number(pageModel.pageIndex || 1);
    const total = Number(pageModel.pageCount || 0);
    if (total <= 1) { host.innerHTML = ""; return; }
    const pages = [];
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);
    if (start > 1) pages.push(1);
    if (start > 2) pages.push("…");
    for (let i = start; i <= end; i++) pages.push(i);
    if (end < total - 1) pages.push("…");
    if (end < total) pages.push(total);
    host.innerHTML = `<nav class="compact-pagination" aria-label="صفحه‌بندی"><button ${current <= 1 ? "disabled" : ""} data-page="${current - 1}" aria-label="صفحه قبل">‹</button>${pages.map(p => p === "…" ? '<span class="pagination-dots">…</span>' : `<button class="${p === current ? "is-active" : ""}" data-page="${p}">${p.toLocaleString("fa-IR")}</button>`).join("")}<button ${current >= total ? "disabled" : ""} data-page="${current + 1}" aria-label="صفحه بعد">›</button></nav>`;
    host.querySelectorAll("button[data-page]:not([disabled])").forEach(btn => btn.addEventListener("click", () => onPage(Number(btn.dataset.page))));
  }
};
