(async function () {
  const user = await window.PanelLayout.init();
  if (!user) return;
  document.getElementById('basicIcon').innerHTML = window.PanelUI.icon('edit','#0d6efd');
  document.getElementById('imageIcon').innerHTML = window.PanelUI.icon('image','#00a8e8');
  document.getElementById('uploadIcon').innerHTML = window.PanelUI.icon('upload','#0d6efd');
  document.getElementById('currentUserMobile').textContent = user.mobileNumber || '-';

  const form = document.getElementById('advertisementForm');
  const category = document.getElementById('adCategory'), province=document.getElementById('adProvince'), city=document.getElementById('adCity');
  const imageInput=document.getElementById('adImages'), gallery=document.getElementById('imageGallery');
  const saveBtn=document.getElementById('saveAdButton'), submitBtn=document.getElementById('submitAdButton');
  const qs=new URLSearchParams(location.search), editID=Number(qs.get('id')||0), isEdit=location.pathname.includes('edit-advertisement');
  let advertisementID=isEdit?editID:0, details=null, selectedFiles=[];

  if(isEdit && !advertisementID){location.href='advertisements.html';return;}
  await initializeSelectors();
  if(isEdit) await loadDetails(); else form.elements.phoneNumber.value=user.mobileNumber||'';

  imageInput.addEventListener('change',()=>{selectedFiles=Array.from(imageInput.files||[]);renderSelectedPreview();});
  form.addEventListener('submit',save);
  submitBtn.addEventListener('click',submitForReview);

  async function initializeSelectors(){
    try{
      const [menu,provinces]=await Promise.all([window.Categories.getMenu(),window.LocationService.getProvinces()]);
      category.innerHTML='<option value="">انتخاب دسته‌بندی</option>'+menu.flatMap(parent=>(parent.children||[]).map(child=>`<option value="${child.advertisementCategoryID}">${window.UI.escapeHtml(parent.categoryName)} — ${window.UI.escapeHtml(child.categoryName)}</option>`)).join('');
      window.LocationService.fillProvinceSelect(province,provinces);
      await window.LocationService.bindDependent(province,city);
    }catch(e){window.UI.showToast(window.Api.normalizeError(e).message,'error')}
  }

  async function loadDetails(){
    gallery.innerHTML=window.PanelUI.loading();
    try{
      const r=await window.Api.get(window.AppConfig.endpoints.customerAdvertisement(advertisementID));details=r.data;
      if(!['Draft','Rejected'].includes(details.statusCode)){
        window.UI.showToast('این آگهی در وضعیت قابل ویرایش نیست.','warning');
        setTimeout(()=>location.href='advertisements.html',650);return;
      }
      form.elements.title.value=details.title||'';form.elements.description.value=details.description||'';form.elements.price.value=details.price??'';form.elements.phoneNumber.value=details.phoneNumber||user.mobileNumber||'';form.elements.isImmediate.checked=Boolean(details.isImmediate);category.value=String(details.advertisementCategoryID||'');province.value=String(details.provinceID||'');
      province.dispatchEvent(new Event('change'));await waitForCity(details.cityID);renderExistingImages(details.images||[]);
      form.querySelector('.rejection-box')?.remove();
      if(details.rejectionReason){const box=document.createElement('div');box.className='rejection-box';box.textContent=`دلیل رد آگهی: ${details.rejectionReason}`;form.prepend(box)}
      submitBtn.classList.remove('d-none');
    }catch(e){gallery.innerHTML=window.PanelUI.error(window.Api.normalizeError(e).message)}
  }

  async function waitForCity(cityID){
    for(let i=0;i<30;i++){if(!city.disabled){city.value=String(cityID||'');return}await new Promise(r=>setTimeout(r,100));}
  }

  function model(){return {title:form.elements.title.value.trim(),description:form.elements.description.value.trim(),price:window.PanelUI.nullableNumber(form.elements.price.value),phoneNumber:form.elements.phoneNumber.value.trim(),advertisementCategoryID:Number(category.value),provinceID:Number(province.value),cityID:Number(city.value),isImmediate:form.elements.isImmediate.checked,seoTitle:null,seoDescription:null,seoKeywords:null,canonicalUrl:null,openGraphImageUrl:null,isIndexable:null,isFollow:null};}

  async function save(event){
    event.preventDefault();if(!form.reportValidity())return;
    window.PanelUI.setButtonBusy(saveBtn,true,isEdit?'در حال ذخیره...':'در حال ایجاد...');
    try{
      let r;
      if(isEdit) r=await window.Api.put(window.AppConfig.endpoints.customerAdvertisement(advertisementID),model());
      else { r=await window.Api.post(window.AppConfig.endpoints.customerAdvertisements,model()); advertisementID=Number(r.data.recordID||r.data.RecordID||0); if(!advertisementID)throw new Error('شناسه آگهی از API دریافت نشد.'); history.replaceState({},'',`edit-advertisement.html?id=${advertisementID}`); }
      window.UI.showToast(window.PanelUI.opMessage(r,isEdit?'تغییرات ذخیره شد.':'پیش‌نویس ایجاد شد.'),'success');
      if(selectedFiles.length) await uploadSelectedFiles();
      submitBtn.classList.remove('d-none');
      if(!isEdit){setTimeout(()=>location.href=`edit-advertisement.html?id=${advertisementID}`,350)}else await loadDetails();
    }catch(e){window.UI.showToast(e.response?window.Api.normalizeError(e).message:(e.message||'خطا در ذخیره آگهی.'),'error')}
    finally{window.PanelUI.setButtonBusy(saveBtn,false)}
  }

  async function uploadSelectedFiles(){
    if(!advertisementID)return;for(let i=0;i<selectedFiles.length;i++){const fd=new FormData();fd.append('file',selectedFiles[i]);fd.append('isMain',String(i===0 && !(details?.images||[]).length));try{await window.Api.upload(window.AppConfig.endpoints.advertisementImages(advertisementID),fd)}catch(e){window.UI.showToast(`آپلود ${selectedFiles[i].name}: ${window.Api.normalizeError(e).message}`,'error')}}selectedFiles=[];imageInput.value='';window.UI.showToast('تصاویر انتخاب‌شده پردازش شدند.','success');
  }

  async function submitForReview(){if(!advertisementID)return; if(!confirm('آگهی برای بررسی مدیر ارسال شود؟'))return;window.PanelUI.setButtonBusy(submitBtn,true,'در حال ارسال...');try{const r=await window.Api.post(window.AppConfig.endpoints.customerAdvertisementSubmit(advertisementID),{});window.UI.showToast(window.PanelUI.opMessage(r),'success');setTimeout(()=>location.href='advertisements.html',450)}catch(e){window.UI.showToast(window.Api.normalizeError(e).message,'error')}finally{window.PanelUI.setButtonBusy(submitBtn,false)}}

  function renderSelectedPreview(){if(!selectedFiles.length){if(!details)gallery.innerHTML='';return}gallery.innerHTML=selectedFiles.map((f,i)=>`<div class="panel-gallery__item"><img src="${URL.createObjectURL(f)}" alt=""><span class="position-absolute bottom-0 start-0 m-1 badge bg-dark">${i===0?'اولویت اول':'جدید'}</span></div>`).join('')}
  function renderExistingImages(images){details=details||{};details.images=images;if(!images.length){gallery.innerHTML=window.PanelUI.empty();return}gallery.innerHTML=images.map(img=>`<div class="panel-gallery__item ${img.isMainImage?'is-main':''}"><img src="${window.UI.escapeHtml(window.UI.mediaUrl(img.thumbnailPath||img.imagePath))}" alt=""><div class="panel-gallery__actions">${img.isMainImage?'':`<button type="button" data-main-image="${img.advertisementImageID}" title="تصویر اصلی">${window.PanelUI.icon('star','#ff9800')}</button>`}<button type="button" data-delete-image="${img.advertisementImageID}" title="حذف">${window.PanelUI.icon('trash','#ef3d45')}</button></div></div>`).join('');gallery.querySelectorAll('[data-main-image]').forEach(b=>b.addEventListener('click',async()=>{try{await window.Api.patch(window.AppConfig.endpoints.advertisementMainImage(advertisementID,b.dataset.mainImage),{});window.UI.showToast('تصویر اصلی تغییر کرد.','success');await loadDetails()}catch(e){window.UI.showToast(window.Api.normalizeError(e).message,'error')}}));gallery.querySelectorAll('[data-delete-image]').forEach(b=>b.addEventListener('click',async()=>{if(!confirm('تصویر حذف شود؟'))return;try{await window.Api.delete(window.AppConfig.endpoints.advertisementImage(advertisementID,b.dataset.deleteImage));window.UI.showToast('تصویر حذف شد.','success');await loadDetails()}catch(e){window.UI.showToast(window.Api.normalizeError(e).message,'error')}}));}
})();
