(async function () {
  const user = await window.PanelLayout.init();
  if (!user) return;
  const host = document.getElementById('customerDashboard');
  const ep = window.AppConfig.endpoints;
  try {
    const [membershipRes, adsRes, favRes, immediateRes, paymentsRes] = await Promise.all([
      window.Api.get(ep.customerMembership).catch(() => ({ data: null })),
      window.Api.get(ep.customerAdvertisements, { params: { PageIndex: 1, PageSize: 4, Sort: 'newest' } }),
      window.Api.get(ep.customerFavorites, { params: { PageIndex: 1, PageSize: 1 } }),
      window.Api.get(ep.customerAdvertisements, { params: { PageIndex: 1, PageSize: 1, IsImmediate: true } }),
      window.Api.get(ep.customerPayments, { params: { PageIndex: 1, PageSize: 4 } }).catch(() => ({ data: { items: [], pageModel: {} } }))
    ]);
    const membership = membershipRes.data;
    const adsPm = window.PanelUI.pageModel(adsRes.data);
    const favPm = window.PanelUI.pageModel(favRes.data);
    const immediatePm = window.PanelUI.pageModel(immediateRes.data);
    const ads = window.PanelUI.items(adsRes.data);
    const payments = window.PanelUI.items(paymentsRes.data);
    const remaining = membership ? membership.remainingAdvertisements : 0;
    const limit = membership ? membership.advertisementLimit : 0;
    const used = membership ? membership.currentAdvertisementCount : 0;
    const percent = limit ? Math.min(100, Math.round((used / limit) * 100)) : 0;
    const recentAds = ads.length ? ads.map(ad => `<div class="recent-list__item"><img src="${window.UI.escapeHtml(window.UI.mediaUrl(ad.thumbnailPath))}" alt=""><div><div class="recent-list__title">${window.UI.escapeHtml(ad.title)}</div><div class="recent-list__meta">${window.UI.escapeHtml(ad.categoryName)} · ${window.UI.escapeHtml(ad.createDatePersian || '')}</div></div>${window.PanelUI.status(ad.statusCode, ad.statusTitle)}</div>`).join('') : window.PanelUI.empty();
    const paymentRows = payments.length ? payments.map(p => `<div class="admin-quick-item"><div><strong>${window.UI.escapeHtml(p.membershipPlanTitle)}</strong><div class="small text-muted">${window.UI.escapeHtml(p.createDatePersian || '')}</div></div><div class="text-start"><strong>${window.UI.formatPrice(p.amount)}</strong><div>${p.isPaid?'<span class="payment-state paid">پرداخت شده</span>':'<span class="payment-state unpaid">ناموفق</span>'}</div></div></div>`).join('') : '<div class="text-muted small">موردی یافت نشد</div>';

    host.innerHTML = `<div class="dashboard-stats">
      ${stat('موجودی تعرفه', remaining.toLocaleString('fa-IR'), 'آگهی باقی‌مانده', 'green', 'card', '#10a43b', membership ? `از پلن ${window.UI.escapeHtml(membership.membershipPlanTitle)}` : 'بدون عضویت فعال')}
      ${stat('کل آگهی‌ها', Number(adsPm.recordCount||0).toLocaleString('fa-IR'), 'آگهی', 'blue', 'ads', '#126ef4', '<a href="advertisements.html">مشاهده آگهی‌ها ←</a>')}
      ${stat('علاقه‌مندی‌ها', Number(favPm.recordCount||0).toLocaleString('fa-IR'), 'آگهی', 'orange', 'heart', '#ff9300', '<a href="favorites.html">مشاهده علاقه‌مندی‌ها ←</a>')}
      ${stat('آگهی‌های فوری', Number(immediatePm.recordCount||0).toLocaleString('fa-IR'), 'آگهی', 'pink', 'ads', '#e61e64', 'فقط آگهی‌های ثبت‌شده توسط شما')}
    </div>
    <div class="dashboard-lower">
      <section class="panel-card"><div class="panel-card__header"><h2>آخرین آگهی‌های شما</h2><a href="advertisements.html">مشاهده همه</a></div><div class="panel-card__body recent-list">${recentAds}</div></section>
      <section class="panel-card"><div class="panel-card__header"><h2>وضعیت تعرفه</h2><a href="membership.html">مدیریت اشتراک</a></div><div class="panel-card__body membership-overview">${membership ? `<div><span class="text-muted">پلن فعال</span><div class="membership-number">${window.UI.escapeHtml(membership.membershipPlanTitle)}</div></div><div class="panel-progress-row"><div class="panel-progress-row__head"><span>مصرف ظرفیت آگهی</span><span>${used.toLocaleString('fa-IR')} از ${limit.toLocaleString('fa-IR')}</span></div><div class="panel-progress"><div class="panel-progress__bar" style="width:${percent}%"></div></div></div><div class="membership-summary"><div><strong>${remaining.toLocaleString('fa-IR')}</strong><span>باقی‌مانده</span></div><div><strong>${window.UI.escapeHtml(membership.startDatePersian)}</strong><span>شروع</span></div><div><strong>${window.UI.escapeHtml(membership.endDatePersian)}</strong><span>پایان</span></div></div>` : `<div class="text-center py-4"><strong>عضویت فعالی ندارید</strong><p class="text-muted mt-2">برای ثبت آگهی یک طرح انتخاب کنید.</p><a class="btn-panel-orange d-inline-block" href="membership.html">مشاهده طرح‌ها</a></div>`}</div></section>
    </div>
    <section class="panel-card mt-4"><div class="panel-card__header"><h2>آخرین تراکنش‌ها</h2><a href="payments.html">مشاهده همه</a></div><div class="panel-card__body">${paymentRows}</div></section>`;
  } catch (error) { host.innerHTML = window.PanelUI.error(window.Api.normalizeError(error).message); }

  function stat(label,value,unit,kind,icon,color,meta){return `<article class="panel-stat-card stat-${kind}"><div class="panel-stat-card__icon">${window.PanelUI.icon(icon,color)}</div><div><div class="panel-stat-card__label">${label}</div><div class="panel-stat-card__value">${value}</div><div class="small text-muted mt-1">${unit}</div><div class="panel-stat-card__meta">${meta}</div></div></article>`;}
})();
