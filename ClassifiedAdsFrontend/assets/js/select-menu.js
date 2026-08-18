window.SelectMenu = (function () {
  const instances = new WeakMap();
  let openInstance = null;

  function iconForSelect(select) {
    const type = select.dataset.icon || "grid";
    const color = select.dataset.iconColor || "#28a9f5";
    return window.UI.icon(type, color);
  }

  function enhance(select) {
    if (!select || instances.has(select)) return instances.get(select);

    const shell = document.createElement("div");
    shell.className = "select-menu";
    const button = document.createElement("button");
    button.type = "button";
    button.className = "select-menu__button";
    button.setAttribute("aria-haspopup", "listbox");
    button.setAttribute("aria-expanded", "false");
    const list = document.createElement("div");
    list.className = "select-menu__list";
    list.setAttribute("role", "listbox");

    select.parentNode.insertBefore(shell, select);
    shell.appendChild(select);
    shell.appendChild(button);
    shell.appendChild(list);
    select.classList.add("select-menu__native");

    function resetListPosition() {
      list.style.position = "";
      list.style.left = "";
      list.style.right = "";
      list.style.width = "";
      list.style.top = "";
      list.style.bottom = "";
      list.style.maxHeight = "";
      shell.classList.remove("select-menu--up");
    }
    function placeList() {
      if (!shell.classList.contains("is-open")) return;
      const btnRect = button.getBoundingClientRect();
      const rootStyles = getComputedStyle(document.documentElement);
      const isPanel = document.body.classList.contains("panel-page");
      const headerH = isPanel
        ? parseFloat(rootStyles.getPropertyValue("--panel-header-height")) || 74
        : parseFloat(rootStyles.getPropertyValue("--header-h")) || 72;
      const isCompact = window.matchMedia("(max-width: 991.98px)").matches;
      const navH = !isPanel && isCompact ? parseFloat(rootStyles.getPropertyValue("--mobile-nav-h")) || 68 : 0;
      const safe = 8;
      const spaceBelow = window.innerHeight - navH - safe - btnRect.bottom;
      const spaceAbove = btnRect.top - headerH - safe;
      const dropUp = spaceBelow < 168 && spaceAbove > spaceBelow;
      const available = Math.max(132, dropUp ? spaceAbove : spaceBelow);
      list.style.maxHeight = `${Math.min(300, available)}px`;
      shell.classList.toggle("select-menu--up", dropUp);
      const panel = shell.closest(".filter-offcanvas");
      if (panel) {
        const panelRect = panel.getBoundingClientRect();
        const gutter = 10;
        const maxW = Math.max(80, panelRect.width - gutter * 2);
        const width = Math.min(btnRect.width, maxW);
        const minLeft = panelRect.left + gutter;
        const maxLeft = panelRect.right - gutter - width;
        const left = Math.min(Math.max(btnRect.left, minLeft), maxLeft);
        list.style.position = "fixed";
        list.style.left = `${left}px`;
        list.style.right = "auto";
        list.style.width = `${width}px`;
        if (dropUp) {
          list.style.top = "auto";
          list.style.bottom = `${window.innerHeight - btnRect.top + 6}px`;
        } else {
          list.style.top = `${btnRect.bottom + 6}px`;
          list.style.bottom = "auto";
        }
      }
    }
    function close() {
      shell.classList.remove("is-open");
      button.setAttribute("aria-expanded", "false");
      resetListPosition();
      if (openInstance === api) openInstance = null;
    }
    function open() {
      if (select.disabled) return;
      if (openInstance && openInstance !== api) openInstance.close();
      shell.classList.add("is-open");
      button.setAttribute("aria-expanded", "true");
      openInstance = api;
      placeList();
    }
    function sync() {
      shell.classList.toggle("is-disabled", select.disabled);
      button.disabled = select.disabled;
      const selected = select.options[select.selectedIndex];
      const visual = selected && selected.dataset.color
        ? `<span class="select-menu__leading" style="--select-accent:${selected.dataset.color}">${window.UI.icon(selected.dataset.icon || "grid", "#fff")}</span>`
        : `<span class="select-menu__leading select-menu__leading--plain">${iconForSelect(select)}</span>`;
      button.innerHTML = `${visual}<span class="select-menu__label">${window.UI.escapeHtml(selected ? selected.text : select.dataset.placeholder || "انتخاب کنید")}</span><span class="select-menu__chevron" aria-hidden="true">${window.UI.icon("chevronDown")}</span>`;
      renderList();
    }
    function renderList() {
      list.innerHTML = "";
      Array.from(select.options).forEach((option, index) => {
        if (option.hidden) return;
        const item = document.createElement("button");
        item.type = "button";
        item.className = "select-menu__option";
        item.setAttribute("role", "option");
        item.setAttribute("aria-selected", String(option.selected));
        item.dataset.value = option.value;
        const level = option.dataset.level || "0";
        item.classList.toggle("is-child", level === "2");
        if (option.dataset.group === "true") item.classList.add("is-group");
        let leading = "";
        if (option.dataset.color) {
          leading = `<span class="select-menu__option-icon" style="--option-color:${option.dataset.color}">${window.UI.icon(option.dataset.icon || "grid", "#fff")}</span>`;
        }
        item.innerHTML = `${leading}<span>${window.UI.escapeHtml(option.text)}</span>${option.selected ? `<span class="select-menu__selected">${window.UI.icon("check", "#1E3E62")}</span>` : ""}`;
        item.addEventListener("click", function () {
          select.selectedIndex = index;
          select.dispatchEvent(new Event("change", { bubbles: true }));
          sync();
          close();
        });
        list.appendChild(item);
      });
    }

    button.addEventListener("click", function () {
      shell.classList.contains("is-open") ? close() : open();
    });
    select.addEventListener("change", sync);
    const observer = new MutationObserver(sync);
    observer.observe(select, { childList: true, subtree: true, attributes: true, attributeFilter: ["disabled"] });

    const api = { close, open, sync, placeList };
    instances.set(select, api);
    sync();
    return api;
  }

  document.addEventListener("click", function (event) {
    if (openInstance && !event.target.closest(".select-menu")) openInstance.close();
  });
  document.addEventListener("keydown", function (event) {
    if (event.key === "Escape" && openInstance) openInstance.close();
  });
  window.addEventListener("resize", function () {
    openInstance?.placeList?.();
  }, { passive: true });
  document.addEventListener("scroll", function (event) {
    if (!openInstance) return;
    const target = event.target;
    if (target instanceof Element && target.closest(".select-menu__list")) return;
    openInstance.placeList?.();
  }, { capture: true, passive: true });

  function enhanceAll(root) {
    (root || document).querySelectorAll("select.js-select-menu").forEach(enhance);
  }

  return { enhance, enhanceAll };
})();
