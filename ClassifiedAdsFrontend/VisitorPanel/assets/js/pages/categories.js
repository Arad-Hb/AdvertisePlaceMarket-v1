(function () {
  const host = document.getElementById("categoriesFull");
  async function load() {
    host.innerHTML = window.UI.loadingMarkup();
    try {
      const categories = await window.Categories.getMenu();
      if (!categories.length) { host.innerHTML = window.UI.emptyMarkup(); return; }
      host.innerHTML = categories.map((parent, index) => {
        const visual = window.UI.categoryVisual(parent.categoryName, index);
        return `<article class="category-group-card"><div class="category-group-card__head"><span style="--cat-color:${visual.color}">${window.UI.icon(visual.iconName, "#fff")}</span><div><h2><a href="${window.Categories.categoryUrl(parent.advertisementCategoryID)}">${window.UI.escapeHtml(parent.categoryName)}</a></h2><small>${Number(parent.advertisementCount || 0).toLocaleString("fa-IR")} آگهی</small></div></div><div class="category-group-card__children">${(parent.children || []).map(child => `<a href="${window.Categories.categoryUrl(child.advertisementCategoryID)}"><span>${window.UI.escapeHtml(child.categoryName)}</span><small>${Number(child.advertisementCount || 0).toLocaleString("fa-IR")}</small></a>`).join("")}</div></article>`;
      }).join("");
    } catch (error) {
      host.innerHTML = window.UI.errorMarkup(window.Api.normalizeError(error).message);
      host.querySelector("[data-retry]")?.addEventListener("click", load, { once: true });
    }
  }
  load();
})();
