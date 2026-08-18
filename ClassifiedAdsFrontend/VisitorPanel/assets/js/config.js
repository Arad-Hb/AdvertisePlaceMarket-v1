window.AppConfig = {
  apiBaseUrl: "http://localhost:5086/api",
  mediaBaseUrl: "http://localhost:5086",
  siteName: "تحلیل داده آگهی",
  tokenStorageKey: "td_agahi_access_token",
  userStorageKey: "td_agahi_current_user",
  pageSize: 9,
  defaultAdvertisementImage: "assets/images/placeholders/advertisement-placeholder.svg",
  defaultAvatar: "assets/images/placeholders/default-avatar.svg",
  endpoints: {
    siteHome: "/site/home",
    siteSettings: "/site/settings",
    categoriesMenu: "/categories/menu",
    categoryDetails: id => `/categories/${id}`,
    categoryBySlug: slug => `/categories/by-slug/${encodeURIComponent(slug)}`,
    provinces: "/provinces",
    provinceDetails: id => `/provinces/${id}`,
    citiesByProvince: id => `/cities/by-province/${id}`,
    cityDetails: id => `/cities/${id}`,
    advertisements: "/advertisements",
    advertisementDetails: id => `/advertisements/${id}`,
    advertisementBySlug: slug => `/advertisements/by-slug/${encodeURIComponent(slug)}`,
    membershipPlans: "/membership-plans",
    membershipPlanDetails: id => `/membership-plans/${id}`,
    login: "/account/login",
    register: "/account/register",
    logout: "/account/logout",
    authenticatedUser: "/account/authenticated-user",
    customerFavorites: "/customer/favorites",
    favoriteByAdvertisement: id => `/customer/favorites/${id}`,
    purchaseMembership: id => `/customer/membership/purchase/${id}`
  },
  demoAccounts: {
    admin: { mobile: "09120000001", password: "Admin@123456" },
    customer: { mobile: "09120000002", password: "Customer@123456" }
  }
};
