(function () {
  const host = document.getElementById("pricingGrid");
  async function load() {
    host.innerHTML = window.UI.loadingMarkup();
    try {
      const response = await window.Api.get(window.AppConfig.endpoints.membershipPlans);
      const plans = Array.isArray(response.data) ? response.data : [];
      if (!plans.length) { host.innerHTML = window.UI.emptyMarkup(); return; }
      host.innerHTML = plans.map((plan, index) => renderPlan(plan, index === Math.min(1, plans.length - 1))).join("");
      host.querySelectorAll("[data-plan-id]").forEach(button => button.addEventListener("click", () => choosePlan(button)));
      const requested = new URLSearchParams(location.search).get("purchasePlanID");
      if (requested && window.Auth.isAuthenticated()) window.UI.showToast("برای فعال‌سازی، پلن موردنظر را دوباره انتخاب کنید.", "info");
    } catch (error) {
      host.innerHTML = window.UI.errorMarkup(window.Api.normalizeError(error).message);
      host.querySelector("[data-retry]")?.addEventListener("click", load, { once: true });
    }
  }
  function renderPlan(plan, recommended) {
    return `<article class="price-card ${recommended ? "is-recommended" : ""}">${recommended ? '<span class="price-card__badge">پیشنهاد مناسب</span>' : ''}<div class="price-card__head"><span class="price-card__icon">${window.UI.icon("briefcase", "#fff")}</span><h2>${window.UI.escapeHtml(plan.title)}</h2><p>${window.UI.escapeHtml(plan.description || "پلن عضویت")}</p><div class="price-card__price">${window.UI.formatPrice(plan.price)}</div></div><ul class="price-card__features"><li><span>مدت اعتبار</span><strong>${Number(plan.durationDays).toLocaleString("fa-IR")} روز</strong></li><li><span>تعداد آگهی مجاز</span><strong>${Number(plan.advertisementLimit).toLocaleString("fa-IR")} آگهی</strong></li><li><span>وضعیت پلن</span><strong class="text-success">فعال</strong></li></ul><button class="btn ${recommended ? "btn-primary" : "btn-soft-primary"} w-100" data-plan-id="${plan.membershipPlanID}" type="button">انتخاب این پلن</button></article>`;
  }
  async function choosePlan(button) {
    const id = button.dataset.planId;
    const customerTarget = `customer/membership.html?planId=${encodeURIComponent(id)}`;
    if (!window.Auth.isAuthenticated()) return window.Auth.redirectToLogin(customerTarget);
    if (!window.Auth.hasRole("Customer")) return window.UI.showToast("انتخاب پلن فقط برای کاربران مشتری امکان‌پذیر است.", "warning");
    location.href = customerTarget;
  }
  load();
})();
