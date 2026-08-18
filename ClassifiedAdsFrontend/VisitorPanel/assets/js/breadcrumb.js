window.Breadcrumb = {
  render: function (host, items) {
    if (!host) return;
    const list = Array.isArray(items) && items.length ? items : [{ title: "خانه", url: "index.html" }];
    host.innerHTML = list.map((item, index) => {
      let url = item.url || "";
      if (url) url = url.replace(/\?category=(\d+)/i, "?AdvertisementCategoryID=$1");
      const body = url ? `<a href="${window.UI.escapeHtml(url)}">${window.UI.escapeHtml(item.title)}</a>` : `<span>${window.UI.escapeHtml(item.title)}</span>`;
      return `${index ? '<i class="breadcrumb-separator">‹</i>' : ''}${body}`;
    }).join("");
  }
};
