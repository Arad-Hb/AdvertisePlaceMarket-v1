window.Categories = (function () {
  let cache = null;

  async function getMenu(force) {
    if (cache && !force) return cache;
    const response = await window.Api.get(window.AppConfig.endpoints.categoriesMenu);
    cache = Array.isArray(response.data) ? response.data : [];
    return cache;
  }

  function categoryUrl(id) {
    return `advertisements.html?AdvertisementCategoryID=${encodeURIComponent(id)}`;
  }

  function fillSelect(select, categories, placeholder) {
    if (!select) return;
    select.innerHTML = `<option value="">${window.UI.escapeHtml(placeholder || "انتخاب دسته‌بندی")}</option>`;
    categories.forEach((parent, parentIndex) => {
      const visual = window.UI.categoryVisual(parent.categoryName, parentIndex);
      const parentOption = document.createElement("option");
      parentOption.value = parent.advertisementCategoryID;
      parentOption.textContent = parent.categoryName;
      parentOption.dataset.group = "true";
      parentOption.dataset.level = "1";
      parentOption.dataset.color = visual.color;
      parentOption.dataset.icon = visual.iconName;
      select.appendChild(parentOption);
      (parent.children || []).forEach(child => {
        const childOption = document.createElement("option");
        childOption.value = child.advertisementCategoryID;
        childOption.textContent = `   ${child.categoryName}`;
        childOption.dataset.level = "2";
        childOption.dataset.color = visual.color;
        childOption.dataset.icon = visual.iconName;
        select.appendChild(childOption);
      });
    });
    if (window.SelectMenu) window.SelectMenu.enhance(select)?.sync();
  }

  function renderMegaMenu(categories) {
    const host = document.getElementById("megaMenuContent");
    if (!host) return;
    host.innerHTML = categories.map((parent, index) => {
      const visual = window.UI.categoryVisual(parent.categoryName, index);
      return `<section class="mega-category">
        <a class="mega-category__title" href="${categoryUrl(parent.advertisementCategoryID)}"><span class="mega-category__icon" style="--cat-color:${visual.color}">${window.UI.icon(visual.iconName, "#fff")}</span><span>${window.UI.escapeHtml(parent.categoryName)}</span></a>
        <div class="mega-category__children">${(parent.children || []).slice(0,7).map(child => `<a href="${categoryUrl(child.advertisementCategoryID)}">${window.UI.escapeHtml(child.categoryName)}</a>`).join("")}</div>
      </section>`;
    }).join("");
  }

  function renderMobileMenu(categories) {
    const host = document.getElementById("mobileCategoryList");
    if (!host) return;
    host.innerHTML = categories.map((parent, index) => {
      const visual = window.UI.categoryVisual(parent.categoryName, index);
      return `<details class="mobile-category"><summary><span class="mobile-category__icon" style="--cat-color:${visual.color}">${window.UI.icon(visual.iconName, "#fff")}</span>${window.UI.escapeHtml(parent.categoryName)}</summary><div>${(parent.children || []).map(child => `<a href="${categoryUrl(child.advertisementCategoryID)}">${window.UI.escapeHtml(child.categoryName)}</a>`).join("")}</div></details>`;
    }).join("");
  }

  return { getMenu, fillSelect, renderMegaMenu, renderMobileMenu, categoryUrl };
})();
