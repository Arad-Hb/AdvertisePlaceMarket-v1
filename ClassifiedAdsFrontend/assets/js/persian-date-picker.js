window.PersianDatePicker = (function () {
  const MONTHS = ["فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"];
  const WEEKDAYS = ["ش", "ی", "د", "س", "چ", "پ", "ج"];
  const DAY_COLORS = ["#ff5a23", "#0d6efd", "#12ae62", "#8b45e6", "#00a8e8", "#ff9800", "#ef3d45"];
  let openPicker = null;

  function fa(value) {
    return String(value).replace(/\d/g, digit => "۰۱۲۳۴۵۶۷۸۹"[digit]);
  }
  function pad(value) {
    return String(value).padStart(2, "0");
  }
  function div(a, b) { return ~~(a / b); }
  function mod(a, b) { return a - ~~(a / b) * b; }
  const breaks = [-61, 9, 38, 199, 426, 686, 756, 818, 1111, 1181, 1210, 1635, 2060, 2097, 2192, 2262, 2324, 2394, 2456, 3178];
  function jalCal(jy, withoutLeap) {
    const gy = jy + 621;
    let leapJ = -14;
    let jp = breaks[0];
    let jump = 0;
    let i = 1;
    for (; i < breaks.length; i += 1) {
      const jm = breaks[i];
      jump = jm - jp;
      if (jy < jm) break;
      leapJ += div(jump, 33) * 8 + div(mod(jump, 33), 4);
      jp = jm;
    }
    let n = jy - jp;
    leapJ += div(n, 33) * 8 + div(mod(n, 33) + 3, 4);
    if (mod(jump, 33) === 4 && jump - n === 4) leapJ += 1;
    const leapG = div(gy, 4) - div((div(gy, 100) + 1) * 3, 4) - 150;
    const march = 20 + leapJ - leapG;
    if (withoutLeap) return { gy, march };
    if (jump - n < 6) n = n - jump + div(jump + 4, 33) * 33;
    let leap = mod(mod(n + 1, 33) - 1, 4);
    if (leap === -1) leap = 4;
    return { leap, gy, march };
  }
  function g2d(gy, gm, gd) {
    let d = div((gy + div(gm - 8, 6) + 100100) * 1461, 4) + div(153 * mod(gm + 9, 12) + 2, 5) + gd - 34840408;
    return d - div(div(gy + 100100 + div(gm - 8, 6), 100) * 3, 4) + 752;
  }
  function d2g(jdn) {
    let j = 4 * jdn + 139361631;
    j = j + div(div(4 * jdn + 183187720, 146097) * 3, 4) * 4 - 3908;
    const i = div(mod(j, 1461), 4) * 5 + 308;
    const gd = div(mod(i, 153), 5) + 1;
    const gm = mod(div(i, 153), 12) + 1;
    const gy = div(j, 1461) - 100100 + div(8 - gm, 6);
    return { gy, gm, gd };
  }
  function j2d(jy, jm, jd) {
    const r = jalCal(jy, true);
    return g2d(r.gy, 3, r.march) + (jm - 1) * 31 - div(jm, 7) * (jm - 7) + jd - 1;
  }
  function d2j(jdn) {
    const gy = d2g(jdn).gy;
    let jy = gy - 621;
    const r = jalCal(jy, false);
    const jdn1f = g2d(gy, 3, r.march);
    let k = jdn - jdn1f;
    if (k >= 0) {
      if (k <= 185) return { jy, jm: 1 + div(k, 31), jd: mod(k, 31) + 1 };
      k -= 186;
    } else {
      jy -= 1;
      k += 179;
      if (r.leap === 1) k += 1;
    }
    return { jy, jm: 7 + div(k, 30), jd: mod(k, 30) + 1 };
  }
  function toJalaali(gy, gm, gd) { return d2j(g2d(gy, gm, gd)); }
  function toGregorian(jy, jm, jd) { return d2g(j2d(jy, jm, jd)); }
  function monthLength(jy, jm) {
    if (jm <= 6) return 31;
    if (jm <= 11) return 30;
    return jalCal(jy, false).leap === 0 ? 30 : 29;
  }
  function parseNative(value, withTime) {
    if (!value) return null;
    const match = withTime
      ? value.match(/^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})/)
      : value.match(/^(\d{4})-(\d{2})-(\d{2})/);
    if (!match) return null;
    const g = { gy: Number(match[1]), gm: Number(match[2]), gd: Number(match[3]) };
    const j = toJalaali(g.gy, g.gm, g.gd);
    return { ...j, hour: withTime ? Number(match[4]) : 0, minute: withTime ? Number(match[5]) : 0 };
  }
  function nativeValue(j, withTime) {
    const g = toGregorian(j.jy, j.jm, j.jd);
    const date = `${g.gy}-${pad(g.gm)}-${pad(g.gd)}`;
    return withTime ? `${date}T${pad(j.hour || 0)}:${pad(j.minute || 0)}` : date;
  }
  function displayValue(j, withTime) {
    const date = `${fa(j.jy)}/${fa(pad(j.jm))}/${fa(pad(j.jd))}`;
    return withTime ? `${date} ${fa(pad(j.hour || 0))}:${fa(pad(j.minute || 0))}` : date;
  }
  function todayParts(withTime) {
    const now = new Date();
    const j = toJalaali(now.getFullYear(), now.getMonth() + 1, now.getDate());
    return { ...j, hour: withTime ? now.getHours() : 0, minute: withTime ? now.getMinutes() : 0 };
  }

  function enhance(input) {
    if (!input || input.dataset.pdp === "1") return;
    const withTime = input.type === "datetime-local";
    input.dataset.pdp = "1";
    input.classList.add("pdp__native");

    const shell = document.createElement("div");
    shell.className = "pdp";
    input.parentNode.insertBefore(shell, input);
    shell.appendChild(input);

    const display = document.createElement("input");
    display.type = "text";
    display.className = `${input.className.replace("pdp__native", "")} pdp__display`.trim();
    display.readOnly = true;
    display.placeholder = input.placeholder || (withTime ? "۱۴۰۴/۰۱/۰۱ ۱۸:۳۰" : "۱۴۰۴/۰۱/۰۱");
    shell.insertBefore(display, input);

    const pop = document.createElement("div");
    pop.className = "pdp__pop";
    shell.appendChild(pop);

    let view = todayParts(withTime);
    let selected = parseNative(input.value, withTime);

    const descriptor = Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, "value");
    Object.defineProperty(input, "value", {
      configurable: true,
      enumerable: true,
      get() { return descriptor.get.call(this); },
      set(next) {
        descriptor.set.call(this, next);
        selected = parseNative(next, withTime);
        syncDisplay();
      }
    });

    function syncDisplay() {
      display.value = selected ? displayValue(selected, withTime) : "";
    }
    function close() {
      shell.classList.remove("is-open");
      if (openPicker === api) openPicker = null;
    }
    function open() {
      if (openPicker && openPicker !== api) openPicker.close();
      view = selected ? { ...selected } : todayParts(withTime);
      render();
      shell.classList.add("is-open");
      openPicker = api;
    }
    function commit(next, keepOpen) {
      selected = next;
      descriptor.set.call(input, nativeValue(next, withTime));
      input.dispatchEvent(new Event("change", { bubbles: true }));
      syncDisplay();
      if (!keepOpen && !withTime) close();
      else render();
    }
    function render() {
      const today = todayParts(false);
      const length = monthLength(view.jy, view.jm);
      const first = toGregorian(view.jy, view.jm, 1);
      const weekday = (new Date(first.gy, first.gm - 1, first.gd).getDay() + 1) % 7;
      const cells = [];
      for (let i = 0; i < weekday; i += 1) cells.push("<span class='pdp__empty'></span>");
      for (let day = 1; day <= length; day += 1) {
        const isToday = today.jy === view.jy && today.jm === view.jm && today.jd === day;
        const isSelected = selected && selected.jy === view.jy && selected.jm === view.jm && selected.jd === day;
        const color = DAY_COLORS[(weekday + day - 1) % DAY_COLORS.length];
        cells.push(`<button type="button" class="pdp__day${isToday ? " is-today" : ""}${isSelected ? " is-selected" : ""}" data-day="${day}" style="--day-color:${color}">${fa(day)}</button>`);
      }
      const timeRow = withTime ? `<div class="pdp__time"><label>ساعت<select data-pdp-hour>${Array.from({ length: 24 }, (_, hour) => `<option value="${hour}" ${hour === (selected?.hour || view.hour || 0) ? "selected" : ""}>${fa(pad(hour))}</option>`).join("")}</select></label><label>دقیقه<select data-pdp-minute>${Array.from({ length: 12 }, (_, i) => { const minute = i * 5; return `<option value="${minute}" ${minute === (selected?.minute || view.minute || 0) ? "selected" : ""}>${fa(pad(minute))}</option>`; }).join("")}</select></label></div>` : "";
      pop.innerHTML = `<div class="pdp__head"><button type="button" data-pdp-nav="-1" aria-label="ماه قبل">‹</button><strong>${MONTHS[view.jm - 1]} ${fa(view.jy)}</strong><button type="button" data-pdp-nav="1" aria-label="ماه بعد">›</button></div><div class="pdp__week">${WEEKDAYS.map((day, i) => `<span style="color:${DAY_COLORS[i]}">${day}</span>`).join("")}</div><div class="pdp__grid">${cells.join("")}</div>${timeRow}<div class="pdp__foot"><button type="button" data-pdp-today>امروز</button><button type="button" data-pdp-clear>پاک کردن</button></div>`;
      pop.querySelector("[data-pdp-nav='-1']").addEventListener("click", () => {
        view.jm -= 1;
        if (view.jm < 1) { view.jm = 12; view.jy -= 1; }
        render();
      });
      pop.querySelector("[data-pdp-nav='1']").addEventListener("click", () => {
        view.jm += 1;
        if (view.jm > 12) { view.jm = 1; view.jy += 1; }
        render();
      });
      pop.querySelectorAll("[data-day]").forEach(button => button.addEventListener("click", () => {
        const hour = withTime ? Number(pop.querySelector("[data-pdp-hour]").value) : 0;
        const minute = withTime ? Number(pop.querySelector("[data-pdp-minute]").value) : 0;
        commit({ jy: view.jy, jm: view.jm, jd: Number(button.dataset.day), hour, minute }, withTime);
      }));
      pop.querySelector("[data-pdp-today]").addEventListener("click", () => commit(todayParts(withTime), withTime));
      pop.querySelector("[data-pdp-clear]").addEventListener("click", () => {
        selected = null;
        descriptor.set.call(input, "");
        input.dispatchEvent(new Event("change", { bubbles: true }));
        syncDisplay();
        close();
      });
      if (withTime) {
        pop.querySelector("[data-pdp-hour]").addEventListener("change", event => {
          if (!selected) return;
          commit({ ...selected, hour: Number(event.target.value) }, true);
        });
        pop.querySelector("[data-pdp-minute]").addEventListener("change", event => {
          if (!selected) return;
          commit({ ...selected, minute: Number(event.target.value) }, true);
        });
      }
    }

    display.addEventListener("click", () => shell.classList.contains("is-open") ? close() : open());
    const api = { close, open, syncDisplay };
    syncDisplay();
    return api;
  }

  function enhanceAll(root) {
    (root || document).querySelectorAll("input[type='date'], input[type='datetime-local']").forEach(enhance);
  }

  document.addEventListener("click", event => {
    if (openPicker && !event.target.closest(".pdp")) openPicker.close();
  });
  document.addEventListener("keydown", event => {
    if (event.key === "Escape" && openPicker) openPicker.close();
  });

  return { enhance, enhanceAll };
})();
