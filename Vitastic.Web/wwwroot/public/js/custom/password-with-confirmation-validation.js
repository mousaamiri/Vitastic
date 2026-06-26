"use strict";
document.addEventListener("DOMContentLoaded", () => {
    const passwordInput = document.querySelector("#password");
    const confirmPasswordInput = document.querySelector("#confirmPassword");

    const passwordInvalidTooltip = passwordInput.parentElement.querySelector(".invalid-tooltip");
    const passwordValidTooltip = passwordInput.parentElement.querySelector(".valid-tooltip");
    const confirmInvalidTooltip = confirmPasswordInput.parentElement.querySelector(".invalid-tooltip");
    const confirmValidTooltip = confirmPasswordInput.parentElement.querySelector(".valid-tooltip");

    const serverPasswordError = document.querySelector("#password-validation");
    const serverConfirmError = document.querySelector("#confirmPassword-validation");

    // ولیدیشن رمز عبور
    function validatePassword() {
        const value = passwordInput.value.trim();
        let message = "";

        if (!value) {
            message = "رمز عبور الزامی است.";
        } else if (value.length < 6) {
            message = "رمز عبور باید حداقل ۶ کاراکتر باشد.";
        } else if (!/[A-Z]/.test(value)) {
            message = "رمز عبور باید حداقل یک حرف بزرگ داشته باشد.";
        } else if (!/[0-9]/.test(value)) {
            message = "رمز عبور باید حداقل یک عدد داشته باشد.";
        }

        if (message) {
            passwordInput.setCustomValidity(message);
            passwordInvalidTooltip.textContent = message;
            passwordInput.classList.add("is-invalid");
            passwordInput.classList.remove("is-valid");
        } else {
            passwordInput.setCustomValidity("");
            passwordInvalidTooltip.textContent = "";
            passwordInput.classList.remove("is-invalid");
            passwordInput.classList.add("is-valid");
        }
    }

    // ولیدیشن تکرار رمز عبور
    function validateConfirmPassword() {
        const value = confirmPasswordInput.value.trim();
        let message = "";

        if (!value) {
            message = "تکرار رمز عبور الزامی است.";
        } else if (value !== passwordInput.value.trim()) {
            message = "رمز عبور و تکرار آن مطابقت ندارند.";
        }

        if (message) {
            confirmPasswordInput.setCustomValidity(message);
            confirmInvalidTooltip.textContent = message;
            confirmPasswordInput.classList.add("is-invalid");
            confirmPasswordInput.classList.remove("is-valid");
        } else {
            confirmPasswordInput.setCustomValidity("");
            confirmInvalidTooltip.textContent = "";
            confirmPasswordInput.classList.remove("is-invalid");
            confirmPasswordInput.classList.add("is-valid");
        }
    }

    // اجرا شدن هنگام تایپ
    passwordInput.addEventListener("input", () => {
        validatePassword();
        validateConfirmPassword(); // برای همزمانی با confirm
    });

    confirmPasswordInput.addEventListener("input", validateConfirmPassword);

    // ست کردن اعتبار اولیه (required) بلافاصله بعد از لود
    validatePassword();
    validateConfirmPassword();

    // خطاهای سمت سرور (ModelState)
    if (serverPasswordError && serverPasswordError.textContent.trim() !== "") {
        passwordInvalidTooltip.textContent = serverPasswordError.textContent;
        passwordInput.classList.add("is-invalid");
    }

    if (serverConfirmError && serverConfirmError.textContent.trim() !== "") {
        confirmInvalidTooltip.textContent = serverConfirmError.textContent;
        confirmPasswordInput.classList.add("is-invalid");
    }
});
