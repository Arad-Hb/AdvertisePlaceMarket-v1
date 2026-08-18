(async function () {
  const user = await window.PanelLayout.init();
  if (!user) return;
  document.getElementById("basicIcon").innerHTML = window.PanelUI.icon("edit", "#0d6efd");
  document.getElementById("imageIcon").innerHTML = window.PanelUI.icon("image", "#00a8e8");
  document.getElementById("uploadIcon").innerHTML = window.PanelUI.icon("upload", "#0d6efd");
  document.getElementById("openImageModal").innerHTML = `${window.PanelUI.icon("add", "#fff")}<span>افزودن عکس</span>`;
  document.getElementById("currentUserMobile").textContent = user.mobileNumber || "-";

  const form = document.getElementById("advertisementForm");
  const category = document.getElementById("adCategory");
  const province = document.getElementById("adProvince");
  const city = document.getElementById("adCity");
  const imageInput = document.getElementById("adImages");
  const gallery = document.getElementById("imageGallery");
  const modalPreview = document.getElementById("modalImagePreview");
  const openImageModal = document.getElementById("openImageModal");
  const confirmImagesButton = document.getElementById("confirmImagesButton");
  const imageModal = bootstrap.Modal.getOrCreateInstance(document.getElementById("imageUploadModal"));
  const imageSection = document.getElementById("imageSection");
  const saveBtn = document.getElementById("saveAdButton");
  const submitBtn = document.getElementById("submitAdButton");
  const qs = new URLSearchParams(location.search);
  const editID = Number(qs.get("id") || 0);
  const isEdit = location.pathname.includes("edit-advertisement");
  let advertisementID = isEdit ? editID : 0;
  let details = null;
  let selectedFiles = [];
  let pendingFiles = [];

  if (isEdit && !advertisementID) {
    location.href = "advertisements.html";
    return;
  }
  await initializeSelectors();
  if (isEdit) await loadDetails();
  else form.elements.phoneNumber.value = user.mobileNumber || "";
  setImageUploadEnabled();

  openImageModal.addEventListener("click", () => {
    if (!canAddImages()) {
      window.UI.showToast("پس از ذخیره پیش‌نویس می‌توانید تصویر اضافه کنید.", "warning");
      return;
    }
    pendingFiles = [];
    imageInput.value = "";
    modalPreview.innerHTML = "";
    imageModal.show();
  });
  imageInput.addEventListener("change", () => {
    pendingFiles = Array.from(imageInput.files || []);
    renderModalPreview();
  });
  confirmImagesButton.addEventListener("click", confirmPendingImages);
  form.addEventListener("submit", save);
  submitBtn.addEventListener("click", submitForReview);

  function canAddImages() {
    if (!advertisementID) return false;
    const status = details?.statusCode;
    return !status || status === "Draft" || status === "Rejected";
  }

  function setImageUploadEnabled() {
    const enabled = canAddImages();
    openImageModal.disabled = !enabled;
    imageSection.classList.toggle("is-locked", !enabled);
    openImageModal.title = enabled ? "" : "پس از ذخیره پیش‌نویس می‌توانید تصویر اضافه کنید.";
  }

  async function initializeSelectors() {
    try {
      const [menu, provinces] = await Promise.all([window.Categories.getMenu(), window.LocationService.getProvinces()]);
      category.innerHTML = '<option value="">انتخاب دسته‌بندی</option>' + menu.flatMap(parent => (parent.children || []).map(child => `<option value="${child.advertisementCategoryID}">${window.UI.escapeHtml(parent.categoryName)} — ${window.UI.escapeHtml(child.categoryName)}</option>`)).join("");
      window.SelectMenu.enhance(category);
      window.LocationService.fillProvinceSelect(province, provinces);
      await window.LocationService.bindDependent(province, city);
    } catch (e) {
      window.UI.showToast(window.Api.normalizeError(e).message, "error");
    }
  }

  async function loadDetails() {
    try {
      const r = await window.Api.get(window.AppConfig.endpoints.customerAdvertisement(advertisementID));
      details = r.data;
      if (!["Draft", "Rejected"].includes(details.statusCode)) {
        window.UI.showToast("این آگهی در وضعیت قابل ویرایش نیست.", "warning");
        setTimeout(() => { location.href = "advertisements.html"; }, 650);
        return;
      }
      form.elements.title.value = details.title || "";
      form.elements.description.value = details.description || "";
      form.elements.price.value = details.price ?? "";
      form.elements.phoneNumber.value = details.phoneNumber || user.mobileNumber || "";
      form.elements.isImmediate.checked = Boolean(details.isImmediate);
      category.value = String(details.advertisementCategoryID || "");
      window.SelectMenu.enhance(category)?.sync();
      province.value = String(details.provinceID || "");
      province.dispatchEvent(new Event("change"));
      await waitForCity(details.cityID);
      selectedFiles = [];
      renderThumbs();
      setImageUploadEnabled();
      form.querySelector(".rejection-box")?.remove();
      if (details.rejectionReason) {
        const box = document.createElement("div");
        box.className = "rejection-box";
        box.textContent = `دلیل رد آگهی: ${details.rejectionReason}`;
        form.prepend(box);
      }
      submitBtn.classList.remove("d-none");
    } catch (e) {
      gallery.innerHTML = window.PanelUI.error(window.Api.normalizeError(e).message);
    }
  }

  async function waitForCity(cityID) {
    for (let i = 0; i < 30; i++) {
      if (!city.disabled) {
        city.value = String(cityID || "");
        return;
      }
      await new Promise(r => setTimeout(r, 100));
    }
  }

  function model() {
    return {
      title: form.elements.title.value.trim(),
      description: form.elements.description.value.trim(),
      price: window.PanelUI.nullableNumber(form.elements.price.value),
      phoneNumber: form.elements.phoneNumber.value.trim(),
      advertisementCategoryID: Number(category.value),
      provinceID: Number(province.value),
      cityID: Number(city.value),
      isImmediate: form.elements.isImmediate.checked,
      seoTitle: null,
      seoDescription: null,
      seoKeywords: null,
      canonicalUrl: null,
      openGraphImageUrl: null,
      isIndexable: null,
      isFollow: null
    };
  }

  async function save(event) {
    event.preventDefault();
    if (!form.reportValidity()) return;
    window.PanelUI.setButtonBusy(saveBtn, true, isEdit ? "در حال ذخیره..." : "در حال ایجاد...");
    try {
      let r;
      if (isEdit) r = await window.Api.put(window.AppConfig.endpoints.customerAdvertisement(advertisementID), model());
      else {
        r = await window.Api.post(window.AppConfig.endpoints.customerAdvertisements, model());
        advertisementID = Number(r.data.recordID || r.data.RecordID || 0);
        if (!advertisementID) throw new Error("شناسه آگهی از API دریافت نشد.");
        history.replaceState({}, "", `edit-advertisement.html?id=${advertisementID}`);
      }
      window.UI.showToast(window.PanelUI.opMessage(r, isEdit ? "تغییرات ذخیره شد." : "پیش‌نویس ایجاد شد."), "success");
      if (selectedFiles.length) await uploadSelectedFiles();
      submitBtn.classList.remove("d-none");
      setImageUploadEnabled();
      if (!isEdit) setTimeout(() => { location.href = `edit-advertisement.html?id=${advertisementID}`; }, 350);
      else await loadDetails();
    } catch (e) {
      window.UI.showToast(e.response ? window.Api.normalizeError(e).message : (e.message || "خطا در ذخیره آگهی."), "error");
    } finally {
      window.PanelUI.setButtonBusy(saveBtn, false);
    }
  }

  async function confirmPendingImages() {
    if (!pendingFiles.length) {
      window.UI.showToast("ابتدا یک تصویر انتخاب کنید.", "warning");
      return;
    }
    selectedFiles = selectedFiles.concat(pendingFiles);
    pendingFiles = [];
    imageInput.value = "";
    modalPreview.innerHTML = "";
    if (advertisementID) {
      window.PanelUI.setButtonBusy(confirmImagesButton, true, "در حال آپلود...");
      try {
        await uploadSelectedFiles();
        imageModal.hide();
        if (isEdit) await loadDetails();
        else renderThumbs();
      } finally {
        window.PanelUI.setButtonBusy(confirmImagesButton, false);
      }
      return;
    }
    imageModal.hide();
    renderThumbs();
  }

  async function uploadSelectedFiles() {
    if (!advertisementID || !selectedFiles.length) return;
    for (let i = 0; i < selectedFiles.length; i++) {
      const fd = new FormData();
      fd.append("file", selectedFiles[i]);
      fd.append("isMain", String(i === 0 && !(details?.images || []).length));
      try {
        await window.Api.upload(window.AppConfig.endpoints.advertisementImages(advertisementID), fd);
      } catch (e) {
        window.UI.showToast(`آپلود ${selectedFiles[i].name}: ${window.Api.normalizeError(e).message}`, "error");
      }
    }
    selectedFiles = [];
    imageInput.value = "";
    window.UI.showToast("تصاویر انتخاب‌شده پردازش شدند.", "success");
  }

  async function submitForReview() {
    if (!advertisementID) return;
    if (!confirm("آگهی برای بررسی مدیر ارسال شود؟")) return;
    window.PanelUI.setButtonBusy(submitBtn, true, "در حال ارسال...");
    try {
      const r = await window.Api.post(window.AppConfig.endpoints.customerAdvertisementSubmit(advertisementID), {});
      window.UI.showToast(window.PanelUI.opMessage(r), "success");
      setTimeout(() => { location.href = "advertisements.html"; }, 450);
    } catch (e) {
      window.UI.showToast(window.Api.normalizeError(e).message, "error");
    } finally {
      window.PanelUI.setButtonBusy(submitBtn, false);
    }
  }

  function renderModalPreview() {
    if (!pendingFiles.length) {
      modalPreview.innerHTML = "";
      return;
    }
    modalPreview.innerHTML = pendingFiles.map((file, index) => `<div class="panel-gallery__item"><img src="${URL.createObjectURL(file)}" alt=""><span class="position-absolute bottom-0 start-0 m-1 badge bg-dark">${index + 1}</span></div>`).join("");
  }

  function renderThumbs() {
    const existing = details?.images || [];
    const existingHtml = existing.map(img => `<div class="panel-gallery__item ${img.isMainImage ? "is-main" : ""}"><img src="${window.UI.escapeHtml(window.UI.mediaUrl(img.thumbnailPath || img.imagePath))}" alt=""><div class="panel-gallery__actions">${img.isMainImage ? "" : `<button type="button" data-main-image="${img.advertisementImageID}" title="تصویر اصلی">${window.PanelUI.icon("star", "#ff9800")}</button>`}<button type="button" data-delete-image="${img.advertisementImageID}" title="حذف">${window.PanelUI.icon("trash", "#ef3d45")}</button></div></div>`).join("");
    const pendingHtml = selectedFiles.map((file, index) => `<div class="panel-gallery__item"><img src="${URL.createObjectURL(file)}" alt=""><div class="panel-gallery__actions"><button type="button" data-remove-pending="${index}" title="حذف">${window.PanelUI.icon("trash", "#ef3d45")}</button></div></div>`).join("");
    gallery.innerHTML = existingHtml + pendingHtml;
    gallery.querySelectorAll("[data-main-image]").forEach(button => button.addEventListener("click", async () => {
      try {
        await window.Api.patch(window.AppConfig.endpoints.advertisementMainImage(advertisementID, button.dataset.mainImage), {});
        window.UI.showToast("تصویر اصلی تغییر کرد.", "success");
        await loadDetails();
      } catch (e) {
        window.UI.showToast(window.Api.normalizeError(e).message, "error");
      }
    }));
    gallery.querySelectorAll("[data-delete-image]").forEach(button => button.addEventListener("click", async () => {
      if (!confirm("تصویر حذف شود؟")) return;
      try {
        await window.Api.delete(window.AppConfig.endpoints.advertisementImage(advertisementID, button.dataset.deleteImage));
        window.UI.showToast("تصویر حذف شد.", "success");
        await loadDetails();
      } catch (e) {
        window.UI.showToast(window.Api.normalizeError(e).message, "error");
      }
    }));
    gallery.querySelectorAll("[data-remove-pending]").forEach(button => button.addEventListener("click", () => {
      selectedFiles.splice(Number(button.dataset.removePending), 1);
      renderThumbs();
    }));
  }
})();
