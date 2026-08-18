window.PanelUI = (function () {
  const colors = {
    dashboard: "#ff9800", ads: "#2196f3", add: "#28c76f", favorite: "#ff2f7d",
    membership: "#8b45e6", payment: "#14a9c9", user: "#ff9800", settings: "#8ea0b4",
    logout: "#ff3b3b", category: "#00a8e8", location: "#18b36b", customers: "#ae3de1",
    banner: "#ff6b3d", site: "#7e57c2", review: "#f5a623", report: "#21b573"
  };

  function icon(name, color) {
    const c = color || "currentColor";
    const p = {
      dashboard:`<svg viewBox="0 0 24 24"><path d="M4 13a8 8 0 1 1 16 0" fill="none" stroke="${c}" stroke-width="1.8"/><path d="m12 13 4-4" stroke="${c}" stroke-width="1.8" stroke-linecap="round"/><circle cx="12" cy="13" r="2" fill="${c}"/><path d="M6 17h12" stroke="${c}" stroke-width="1.8" stroke-linecap="round"/></svg>`,
      ads:`<svg viewBox="0 0 24 24"><path d="M6 3h9l4 4v14H6z" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M15 3v5h5M9 12h7M9 16h7" fill="none" stroke="${c}" stroke-width="1.7" stroke-linecap="round"/></svg>`,
      add:`<svg viewBox="0 0 24 24"><path d="M12 5v14M5 12h14" stroke="${c}" stroke-width="2" stroke-linecap="round"/></svg>`,
      heart:`<svg viewBox="0 0 24 24"><path d="M20.8 4.7a5.3 5.3 0 0 0-7.5 0L12 6l-1.3-1.3a5.3 5.3 0 0 0-7.5 7.5L12 21l8.8-8.8a5.3 5.3 0 0 0 0-7.5Z" fill="none" stroke="${c}" stroke-width="1.8"/></svg>`,
      diamond:`<svg viewBox="0 0 24 24"><path d="M4 8 8 3h8l4 5-8 13L4 8Z" fill="none" stroke="${c}" stroke-width="1.7"/><path d="m4 8 8 4 8-4M8 3l4 9 4-9" fill="none" stroke="${c}" stroke-width="1.5"/></svg>`,
      card:`<svg viewBox="0 0 24 24"><rect x="3" y="5" width="18" height="14" rx="2" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M3 9h18M7 15h4" stroke="${c}" stroke-width="1.7" stroke-linecap="round"/></svg>`,
      user:`<svg viewBox="0 0 24 24"><circle cx="12" cy="8" r="3.4" fill="none" stroke="${c}" stroke-width="1.8"/><path d="M5 20c.7-4 3-6 7-6s6.3 2 7 6" fill="none" stroke="${c}" stroke-width="1.8" stroke-linecap="round"/></svg>`,
      gear:`<svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="3" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M12 2v3M12 19v3M2 12h3M19 12h3M5 5l2 2M17 17l2 2M19 5l-2 2M7 17l-2 2" fill="none" stroke="${c}" stroke-width="1.7" stroke-linecap="round"/></svg>`,
      logout:`<svg viewBox="0 0 24 24"><path d="M10 4H5v16h5M14 8l4 4-4 4M9 12h9" fill="none" stroke="${c}" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/></svg>`,
      home:`<svg viewBox="0 0 24 24"><path d="m4 11 8-7 8 7v9h-6v-6h-4v6H4v-9Z" fill="none" stroke="${c}" stroke-width="1.7" stroke-linejoin="round"/></svg>`,
      menu:`<svg viewBox="0 0 24 24"><path d="M4 7h16M4 12h16M4 17h16" fill="none" stroke="${c}" stroke-width="2" stroke-linecap="round"/></svg>`,
      bell:`<svg viewBox="0 0 24 24"><path d="M6 16h12l-1.5-2V9a4.5 4.5 0 0 0-9 0v5L6 16Z" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M10 19h4" stroke="${c}" stroke-width="1.7" stroke-linecap="round"/></svg>`,
      help:`<svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="9" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M9.8 9a2.3 2.3 0 1 1 3.3 2.1c-.8.4-1.1.9-1.1 1.9M12 17h.01" fill="none" stroke="${c}" stroke-width="1.8" stroke-linecap="round"/></svg>`,
      mail:`<svg viewBox="0 0 24 24"><rect x="3" y="5" width="18" height="14" rx="2" fill="none" stroke="${c}" stroke-width="1.7"/><path d="m4 7 8 6 8-6" fill="none" stroke="${c}" stroke-width="1.7"/></svg>`,
      edit:`<svg viewBox="0 0 24 24"><path d="M4 20h4L19 9l-4-4L4 16v4Z" fill="none" stroke="${c}" stroke-width="1.7"/><path d="m13 7 4 4" stroke="${c}" stroke-width="1.7"/></svg>`,
      trash:`<svg viewBox="0 0 24 24"><path d="M5 7h14M9 7V4h6v3M8 10v7M12 10v7M16 10v7M6 7l1 14h10l1-14" fill="none" stroke="${c}" stroke-width="1.7" stroke-linecap="round"/></svg>`,
      eye:`<svg viewBox="0 0 24 24"><path d="M3 12s3-5 9-5 9 5 9 5-3 5-9 5-9-5-9-5Z" fill="none" stroke="${c}" stroke-width="1.7"/><circle cx="12" cy="12" r="2.3" fill="none" stroke="${c}" stroke-width="1.7"/></svg>`,
      send:`<svg viewBox="0 0 24 24"><path d="m3 11 18-8-7 18-3-7-8-3Z" fill="none" stroke="${c}" stroke-width="1.7" stroke-linejoin="round"/><path d="m11 14 5-5" stroke="${c}" stroke-width="1.7"/></svg>`,
      upload:`<svg viewBox="0 0 24 24"><path d="M12 16V5M8 9l4-4 4 4" fill="none" stroke="${c}" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/><path d="M5 14v5h14v-5" fill="none" stroke="${c}" stroke-width="1.7"/></svg>`,
      star:`<svg viewBox="0 0 24 24"><path d="m12 3 2.7 5.5 6.1.9-4.4 4.3 1 6.1L12 17l-5.4 2.8 1-6.1-4.4-4.3 6.1-.9L12 3Z" fill="none" stroke="${c}" stroke-width="1.7"/></svg>`,
      search:`<svg viewBox="0 0 24 24"><circle cx="11" cy="11" r="6.5" fill="none" stroke="${c}" stroke-width="1.8"/><path d="m16 16 4 4" stroke="${c}" stroke-width="1.8" stroke-linecap="round"/></svg>`,
      filter:`<svg viewBox="0 0 24 24"><path d="M4 5h16l-6 7v5l-4 2v-7L4 5Z" fill="none" stroke="${c}" stroke-width="1.7" stroke-linejoin="round"/></svg>`,
      folder:`<svg viewBox="0 0 24 24"><path d="M3 6h7l2 2h9v11H3z" fill="none" stroke="${c}" stroke-width="1.7" stroke-linejoin="round"/></svg>`,
      map:`<svg viewBox="0 0 24 24"><path d="m3 6 6-3 6 3 6-3v15l-6 3-6-3-6 3V6Z" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M9 3v15M15 6v15" stroke="${c}" stroke-width="1.5"/></svg>`,
      users:`<svg viewBox="0 0 24 24"><circle cx="9" cy="8" r="3" fill="none" stroke="${c}" stroke-width="1.7"/><circle cx="17" cy="9" r="2.2" fill="none" stroke="${c}" stroke-width="1.5"/><path d="M3 20c.6-4 2.6-6 6-6s5.4 2 6 6M15 15c2.7.2 4.4 1.8 5 5" fill="none" stroke="${c}" stroke-width="1.7" stroke-linecap="round"/></svg>`,
      image:`<svg viewBox="0 0 24 24"><rect x="3" y="4" width="18" height="16" rx="2" fill="none" stroke="${c}" stroke-width="1.7"/><circle cx="9" cy="9" r="2" fill="none" stroke="${c}" stroke-width="1.5"/><path d="m5 18 5-5 3 3 2-2 4 4" fill="none" stroke="${c}" stroke-width="1.7"/></svg>`,
      check:`<svg viewBox="0 0 24 24"><path d="m5 12 4 4L19 6" fill="none" stroke="${c}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>`,
      close:`<svg viewBox="0 0 24 24"><path d="m6 6 12 12M18 6 6 18" stroke="${c}" stroke-width="2" stroke-linecap="round"/></svg>`,
      lock:`<svg viewBox="0 0 24 24"><rect x="5" y="10" width="14" height="10" rx="2" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M8 10V7a4 4 0 0 1 8 0v3" fill="none" stroke="${c}" stroke-width="1.7"/></svg>`,
      globe:`<svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="9" fill="none" stroke="${c}" stroke-width="1.7"/><path d="M3 12h18M12 3c2.6 2.6 4 5.6 4 9s-1.4 6.4-4 9c-2.6-2.6-4-5.6-4-9s1.4-6.4 4-9Z" fill="none" stroke="${c}" stroke-width="1.5"/></svg>`
    };
    return p[name] || p.dashboard;
  }

  function loading() {
    return `<div class="panel-loading"><div><div class="panel-loading__dots"><span></span><span></span><span></span></div><div>اطلاعات در حال بارگذاری می باشند..</div></div></div>`;
  }
  function empty() {
    return `<div class="panel-empty"><div class="panel-empty__skeleton"><div class="panel-empty__lines"><i></i><i></i><i></i></div><strong>موردی یافت نشد</strong></div></div>`;
  }
  function error(message) {
    return `<div class="panel-empty"><div><strong class="text-danger">دریافت اطلاعات انجام نشد</strong><div class="text-muted mt-2">${window.UI.escapeHtml(message || "لطفاً دوباره تلاش کنید.")}</div></div></div>`;
  }
  function status(code, title) {
    return `<span class="status-pill status-${window.UI.escapeHtml(code || "Draft")}">${window.UI.escapeHtml(title || code || "-")}</span>`;
  }
  function pageModel(data) {
    return data && (data.pageModel || data.PageModel) ? (data.pageModel || data.PageModel) : { pageIndex:1,pageSize:10,recordCount:0,pageCount:0 };
  }
  function items(data) { return data && (data.items || data.Items) ? (data.items || data.Items) : []; }
  function opMessage(response, fallback) { return response && response.data && (response.data.message || response.data.Message) || fallback || "عملیات با موفقیت انجام شد."; }
  function confirmAction(message) { return window.confirm(message); }
  function setButtonBusy(button, busy, text) {
    if (!button) return;
    if (busy) { button.dataset.originalHtml = button.innerHTML; button.disabled = true; button.innerHTML = `<span class="spinner-border spinner-border-sm"></span> ${text || "در حال انجام..."}`; }
    else { button.disabled = false; if (button.dataset.originalHtml) button.innerHTML = button.dataset.originalHtml; }
  }
  function renderPagination(host, pageModel, onPage) {
    if (!host) return;
    const page = Number(pageModel.pageIndex || pageModel.PageIndex || 1);
    const count = Number(pageModel.pageCount || pageModel.PageCount || 0);
    if (count <= 1) { host.innerHTML = ""; return; }
    const start = Math.max(1, page - 2), end = Math.min(count, page + 2);
    let html = `<nav class="d-flex justify-content-center mt-4"><ul class="pagination pagination-sm gap-1">`;
    html += `<li class="page-item ${page<=1?'disabled':''}"><button class="page-link" data-page="${page-1}">‹</button></li>`;
    for (let i=start;i<=end;i++) html += `<li class="page-item ${i===page?'active':''}"><button class="page-link" data-page="${i}">${i.toLocaleString('fa-IR')}</button></li>`;
    html += `<li class="page-item ${page>=count?'disabled':''}"><button class="page-link" data-page="${page+1}">›</button></li></ul></nav>`;
    host.innerHTML = html;
    host.querySelectorAll('[data-page]').forEach(b => b.addEventListener('click', () => { if (!b.closest('.disabled')) onPage(Number(b.dataset.page)); }));
  }
  function serializeForm(form) {
    const data = {};
    new FormData(form).forEach((value,key) => { if (value !== "") data[key] = value; });
    form.querySelectorAll('input[type="checkbox"]').forEach(input => data[input.name] = input.checked);
    return data;
  }
  function nullableNumber(v) { return v === "" || v === null || v === undefined ? null : Number(v); }

  return { colors, icon, loading, empty, error, status, pageModel, items, opMessage, confirmAction, setButtonBusy, renderPagination, serializeForm, nullableNumber };
})();
