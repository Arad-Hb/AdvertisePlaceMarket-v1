window.SiteData = (function () {
  let settingsCache = null;
  let homeCache = null;
  async function getSettings(force) {
    if (settingsCache && !force) return settingsCache;
    const response = await window.Api.get(window.AppConfig.endpoints.siteSettings);
    settingsCache = response.data || {};
    return settingsCache;
  }
  async function getHome(force) {
    if (homeCache && !force) return homeCache;
    const response = await window.Api.get(window.AppConfig.endpoints.siteHome);
    homeCache = response.data || {};
    if (homeCache.siteSetting) settingsCache = homeCache.siteSetting;
    return homeCache;
  }
  return { getSettings, getHome };
})();
