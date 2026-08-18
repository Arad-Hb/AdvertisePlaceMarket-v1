window.Favorites = (function () {
  async function toggle(button, advertisementID) {
    if (!window.Auth.isAuthenticated()) {
      const current = new URL(location.href);
      current.searchParams.set("favorite", advertisementID);
      window.Auth.redirectToLogin(current.pathname.split("/").pop() + current.search);
      return;
    }
    try {
      const isActive = button.classList.contains("is-active");
      if (isActive) {
        await window.Api.delete(window.AppConfig.endpoints.favoriteByAdvertisement(advertisementID));
        button.classList.remove("is-active");
        window.UI.showToast("از علاقه‌مندی‌ها حذف شد.", "success");
      } else {
        await window.Api.post(window.AppConfig.endpoints.favoriteByAdvertisement(advertisementID), {});
        button.classList.add("is-active");
        window.UI.showToast("در علاقه‌مندی‌ها ذخیره شد.", "success");
      }
    } catch (error) {
      const e = window.Api.normalizeError(error);
      if (e.status === 401) return window.Auth.redirectToLogin(location.pathname.split("/").pop() + location.search);
      window.UI.showToast(e.message, "error");
    }
  }
  function bind(root) {
    (root || document).querySelectorAll("[data-favorite-id]").forEach(button => {
      if (button.dataset.favoriteBound) return;
      button.dataset.favoriteBound = "1";
      button.addEventListener("click", function (event) {
        event.preventDefault();
        event.stopPropagation();
        toggle(button, button.dataset.favoriteId);
      });
    });
  }
  async function handlePending() {
    const q = new URLSearchParams(location.search);
    const id = q.get("favorite");
    if (!id || !window.Auth.isAuthenticated()) return;
    try {
      await window.Api.post(window.AppConfig.endpoints.favoriteByAdvertisement(id), {});
      window.UI.showToast("آگهی در علاقه‌مندی‌ها ذخیره شد.", "success");
      q.delete("favorite");
      history.replaceState({}, "", `${location.pathname}${q.toString() ? `?${q}` : ""}`);
    } catch { /* normal page remains usable */ }
  }
  return { bind, handlePending, toggle };
})();
