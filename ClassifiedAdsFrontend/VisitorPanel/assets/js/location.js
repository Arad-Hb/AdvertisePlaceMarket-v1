window.LocationService = (function () {
  let provincesCache = null;

  async function getProvinces() {
    if (provincesCache) return provincesCache;
    const response = await window.Api.get(window.AppConfig.endpoints.provinces);
    provincesCache = Array.isArray(response.data) ? response.data : [];
    return provincesCache;
  }
  async function getCities(provinceID) {
    if (!provinceID) return [];
    const response = await window.Api.get(window.AppConfig.endpoints.citiesByProvince(provinceID));
    return Array.isArray(response.data) ? response.data : [];
  }
  function fillProvinceSelect(select, provinces) {
    if (!select) return;
    select.innerHTML = '<option value="">انتخاب استان</option>' + provinces.map(p => `<option value="${p.provinceID}">${window.UI.escapeHtml(p.provinceName)}</option>`).join("");
    window.SelectMenu?.enhance(select)?.sync();
  }
  function resetCity(select, text) {
    if (!select) return;
    select.disabled = true;
    select.innerHTML = `<option value="">${window.UI.escapeHtml(text || "ابتدا استان را انتخاب کنید")}</option>`;
    window.SelectMenu?.enhance(select)?.sync();
  }
  async function bindDependent(provinceSelect, citySelect, selectedCityID) {
    if (!provinceSelect || !citySelect) return;
    resetCity(citySelect);
    provinceSelect.addEventListener("change", async function () {
      const provinceID = this.value;
      resetCity(citySelect, provinceID ? "اطلاعات در حال بارگذاری می باشند.." : "ابتدا استان را انتخاب کنید");
      if (!provinceID) return;
      try {
        const cities = await getCities(provinceID);
        citySelect.innerHTML = '<option value="">انتخاب شهر</option>' + cities.map(c => `<option value="${c.cityID}">${window.UI.escapeHtml(c.cityName)}</option>`).join("");
        citySelect.disabled = false;
        if (selectedCityID) citySelect.value = String(selectedCityID);
        window.SelectMenu?.enhance(citySelect)?.sync();
      } catch (error) {
        resetCity(citySelect, "دریافت شهرها ناموفق بود");
        window.UI.showToast(window.Api.normalizeError(error).message, "error");
      }
    });
    if (provinceSelect.value) provinceSelect.dispatchEvent(new Event("change"));
  }
  return { getProvinces, getCities, fillProvinceSelect, bindDependent, resetCity };
})();
