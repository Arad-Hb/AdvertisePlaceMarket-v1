window.SelectMenu = (function () {
  const instances = new WeakMap();
  let openInstance = null;

  function iconForSelect(select) {
    const type = select.dataset.icon || "grid";
    const color = select.dataset.iconColor || "#4DB7FE";
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

    function close() {
      shell.classList.remove("is-open");
      button.setAttribute("aria-expanded", "false");
      if (openInstance === api) openInstance = null;
    }
    function open() {
      if (select.disabled) return;
      if (openInstance && openInstance !== api) openInstance.close();
      shell.classList.add("is-open");
      button.setAttribute("aria-expanded", "true");
      openInstance = api;
    }
    function sync() {
      shell.classList.toggle("is-disabled", select.disabled);
      button.disabled = select.disabled;
      const selected = select.options[select.selectedIndex];
      const visual = selected && selected.dataset.color
        ? `<span class="select-menu__leading" style="--select-accent:${selected.dataset.color}">${window.UI.icon(selected.dataset.icon || "grid", "#fff")}</span>`
        : `<span class="select-menu__leading select-menu__leading--plain">${iconForSelect(select)}</span>`;
      button.innerHTML = `${visual}<span class="select-menu__label">${window.UI.escapeHtml(selected ? selected.text : select.dataset.placeholder || "انتخاب کنید")}</span><span class="select-menu__chevron">⌄</span>`;
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
        item.innerHTML = `${leading}<span>${window.UI.escapeHtml(option.text)}</span>${option.selected ? `<span class="select-menu__selected">${window.UI.icon("check", "#1B70CC")}</span>` : ""}`;
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

    const api = { close, open, sync };
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

  function enhanceAll(root) {
    (root || document).querySelectorAll("select.js-select-menu").forEach(enhance);
  }

  return { enhance, enhanceAll };
})();
