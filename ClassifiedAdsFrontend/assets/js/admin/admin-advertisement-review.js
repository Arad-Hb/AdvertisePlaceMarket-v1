(async function () {
  if (!await window.PanelLayout.init()) return;
  const host = document.getElementById("adminReviewHost");
  const id = Number(new URLSearchParams(location.search).get("id") || 0);
  if (!id) {
    location.href = "advertisements.html";
    return;
  }
  async function load() {
    host.innerHTML = window.PanelUI.loading();
    try {
      const r = await window.Api.get(window.AppConfig.endpoints.adminAdvertisement(id));
      window.AdminAdReview.renderPage(host, r.data, id, load);
    } catch (e) {
      host.innerHTML = window.PanelUI.error(window.Api.normalizeError(e).message);
    }
  }
  load();
})();
