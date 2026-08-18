(function () {
  const card = document.getElementById("authCard");
  const form = document.getElementById("authForm");
  const title = document.getElementById("authTitle");
  const subtitle = document.getElementById("authSubtitle");
  const submit = document.getElementById("authSubmit");
  const loginButton = document.getElementById("showLogin");
  const registerButton = document.getElementById("showRegister");
  let mode = new URLSearchParams(location.search).get("mode") === "register" ? "register" : "login";

  function setMode(next) {
    mode = next;
    card.dataset.mode = mode;
    loginButton.classList.toggle("is-active", mode === "login");
    registerButton.classList.toggle("is-active", mode === "register");
    title.textContent = mode === "login" ? "ورود به حساب کاربری" : "ساخت حساب کاربری";
    subtitle.textContent = mode === "login" ? "شماره موبایل و رمز عبور خود را وارد کنید" : "ثبت نام فقط برای کاربران Customer انجام می‌شود";
    submit.innerHTML = `${window.UI.icon(mode === "login" ? "lock" : "user", "#fff")}<span>${mode === "login" ? "ورود به حساب" : "ثبت نام"}</span>`;
    ["firstName", "lastName", "confirmPassword"].forEach(name => { if (form.elements[name]) form.elements[name].required = mode === "register"; });
    const url = new URL(location.href);
    if (mode === "register") url.searchParams.set("mode", "register"); else url.searchParams.delete("mode");
    history.replaceState({}, "", url);
  }

  function addInputIcons() {
    const map = { firstName:["user","#7e57c2"], lastName:["user","#15b6cd"], email:["mail","#ff8a1f"], mobileNumber:["phone","#25c46a"], password:["lock","#1B70CC"], confirmPassword:["lock","#ef5350"] };
    Object.entries(map).forEach(([name, info]) => {
      const input = form.elements[name]; if (!input) return;
      const shell = input.closest(".auth-input-shell");
      if (shell) shell.querySelector(".auth-input-icon").innerHTML = window.UI.icon(info[0], info[1]);
    });
  }

  loginButton.addEventListener("click", () => setMode("login"));
  registerButton.addEventListener("click", () => setMode("register"));
  document.querySelectorAll("[data-password-toggle]").forEach(button => button.addEventListener("click", function () {
    const target = document.getElementById(this.dataset.passwordToggle);
    target.type = target.type === "password" ? "text" : "password";
  }));

  form.addEventListener("submit", async function (event) {
    event.preventDefault();
    submit.disabled = true;
    const original = submit.innerHTML;
    submit.innerHTML = `<span class="button-spinner"></span><span>لطفاً صبر کنید...</span>`;
    try {
      const fd = new FormData(form);
      if (mode === "login") {
        await window.Auth.login({ mobileNumber: fd.get("mobileNumber"), password: fd.get("password"), rememberMe: fd.get("rememberMe") === "on" });
        window.UI.showToast("ورود با موفقیت انجام شد.", "success");
        const returnUrl = new URLSearchParams(location.search).get("returnUrl");
        setTimeout(() => location.href = returnUrl || window.Auth.dashboardUrl(), 380);
      } else {
        if (fd.get("password") !== fd.get("confirmPassword")) throw new Error("رمز عبور و تکرار آن یکسان نیستند.");
        const model = { firstName: fd.get("firstName"), lastName: fd.get("lastName"), mobileNumber: fd.get("mobileNumber"), email: fd.get("email") || null, password: fd.get("password"), confirmPassword: fd.get("confirmPassword") };
        const response = await window.Auth.register(model);
        window.UI.showToast(response.message || "ثبت نام با موفقیت انجام شد. اکنون وارد شوید.", "success");
        form.reset(); setMode("login");
      }
    } catch (error) {
      window.UI.showToast(error && error.response ? window.Api.normalizeError(error).message : (error.message || "خطا در انجام عملیات."), "error");
    } finally {
      submit.disabled = false; submit.innerHTML = original; setMode(mode);
    }
  });

  const admin = window.AppConfig.demoAccounts.admin, customer = window.AppConfig.demoAccounts.customer;
  document.getElementById("demoAccounts").innerHTML = `<div><strong>دمو مدیر</strong><span>${admin.mobile}</span><span>${admin.password}</span></div><div><strong>دمو کاربر</strong><span>${customer.mobile}</span><span>${customer.password}</span></div>`;
  setMode(mode); addInputIcons();
})();
